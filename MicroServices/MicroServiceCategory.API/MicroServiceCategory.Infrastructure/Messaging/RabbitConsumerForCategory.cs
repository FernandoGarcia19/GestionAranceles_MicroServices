using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using MicroServiceCategory.Application.Services;
using MicroServiceCategory.Domain.Entities;
using MicroServiceCategory.Domain.Interfaces;
using MicroServiceCategory.Domain.Patterns;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroServiceCategory.Infrastructure.Messaging;


public class RabbitConsumerForCategory: BackgroundService
{
    private readonly IConnection _connection;   
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly IServiceScopeFactory  _scopeFactory;
    private readonly ILogger<RabbitConsumerForCategory> _logger;
    const string queueName = "category.queue";

    public RabbitConsumerForCategory(IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitConsumerForCategory> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],  
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"],
            DispatchConsumersAsync = true
        };
        
        _exchange = configuration["RabbitMQ:Exchange"];
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
        
        _channel.QueueDeclare(queueName,true, false, false);
        _channel.QueueBind(queueName, _exchange, "payment.created");
        
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnPaymentMessageRecieved;
        _channel.BasicConsume(queueName, true,  consumer);
        return Task.CompletedTask;
    }

    private async Task OnPaymentMessageRecieved(object sender, BasicDeliverEventArgs eventArgs)
    {
        var jsonString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {   
            using var jsonData = JsonDocument.Parse(jsonString);
            var root = jsonData.RootElement;
            IEnumerable<int> paymentCategories =  root.GetProperty("categoryIds").EnumerateArray().Select( element => element.GetInt32() );
            IEnumerable<int> paymentIncrements =  root.GetProperty("categoryIncrements").EnumerateArray().Select( element => element.GetInt32() );
            int paymentId = root.GetProperty("paymentId").GetInt32();
            
            // Payment.Insert(categories, quantities) -> Category.Update(categories, quantities) -> Payment(Ok, NoOk), NoOK? Payment.Delete() 
            using var scope = _scopeFactory.CreateScope();

            CategoryService categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
            IEventPublisher eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var categoryDataPairs =  paymentCategories.Zip(paymentIncrements);
            foreach (var (categoryId, categoryIncrement) in categoryDataPairs)
            {
                Result<int> result =  await categoryService.IncrementNumberOfInserts(categoryId, categoryIncrement);
                if (result.IsFailure)
                {
                    Result<Category> resultCategory = await categoryService.SelectById(categoryId);
                    var failedEventData = new
                    {
                        paymentId,
                        categoryId,
                        categoryName = resultCategory.Value.Name,
                        categoryDescription = resultCategory.Value.Description,
                    };
                    
                    await eventPublisher.PublishAsync( "category.failed_increment", failedEventData );
                }
            }
            
            var okEventData = new
            {
                PaymentId = paymentId,
                CategoryIds = paymentCategories.ToArray(),
                CategoryIncrements = paymentIncrements.ToArray(),
            };
            
            await eventPublisher.PublishAsync("category.increment_updated", okEventData);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}