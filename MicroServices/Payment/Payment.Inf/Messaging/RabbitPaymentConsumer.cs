using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payment.App.Service;
using Payment.Dom.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payment.Inf.Messaging;

public class RabbitPaymentConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly ILogger<RabbitPaymentConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    // Payment.Insert(status=1, sagaStatus=PENDING) -> Category.Update() -> ( Ok? sagaStatus=OK ) ? (NoOk? Payment.Delete(), sagaStatus=FAILED)
    // Payment.sagaStatus = PENDING | OK | FAILED
    const string queueName = "payment.queue";

    public RabbitPaymentConsumer(IConfiguration configuration,
        ILogger<RabbitPaymentConsumer> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory =  serviceScopeFactory;
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = configuration["Rabbit:HostName"],
            Password = configuration["Rabbit:Password"],
            UserName = configuration["Rabbit:UserName"],
            DispatchConsumersAsync = true
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
        
        _channel.QueueDeclare(queueName, true, false, false);
        
        _channel.QueueBind(queueName, _exchangeName, "category.failed_increment");
        _channel.QueueBind(queueName, _exchangeName, "category.increment_updated");
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private async Task OnCategoryMessageRecieved(object sender, BasicDeliverEventArgs eventArgs)
    {
        var jsonData = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            var serviceFactoryScope = _serviceScopeFactory.CreateScope();
            IEventPublisher eventPublisher = serviceFactoryScope.ServiceProvider.GetRequiredService<IEventPublisher>(); 
            PaymentService  paymentService = serviceFactoryScope.ServiceProvider.GetRequiredService<PaymentService>();
            
            using var jsonDoc = JsonDocument.Parse(jsonData);
            var jsonRoot =  jsonDoc.RootElement;
            var routingKey = eventArgs.RoutingKey;
            int paymentId = jsonRoot.GetProperty("paymentId").GetInt32();
            if (routingKey == "category.failed_increment")
            {
                paymentService.UpdateSagaStatus(new Dom.Model.Payment
                    { Id = paymentId, SagaStatus = (int)Dom.Model.PaymentSagaStatus.FAILED });
                paymentService.Delete( new Dom.Model.Payment
                {
                    Id = paymentId, 
                    UpdateDate = DateTime.Now
                } );
            }
            else if (routingKey == "category.increment_updated")
            {
                paymentService.UpdateSagaStatus(new Dom.Model.Payment
                {
                    Id = paymentId,
                    SagaStatus = (int)Dom.Model.PaymentSagaStatus.COMPLETED
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}