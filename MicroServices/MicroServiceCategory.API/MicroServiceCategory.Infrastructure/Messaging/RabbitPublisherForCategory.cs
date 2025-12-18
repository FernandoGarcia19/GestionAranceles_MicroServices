using System.Text.Json;
using MicroServiceCategory.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MicroServiceCategory.Infrastructure.Messaging;

public class RabbitPublisherForCategory : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly ILogger<RabbitPublisherForCategory> _logger;

    public RabbitPublisherForCategory(IConfiguration configuration, ILogger<RabbitPublisherForCategory> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory()
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
    }
    
    public Task PublishAsync(string routingKey, object data)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(data);
        var properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        _channel.BasicPublish(_exchange, routingKey, properties, body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}