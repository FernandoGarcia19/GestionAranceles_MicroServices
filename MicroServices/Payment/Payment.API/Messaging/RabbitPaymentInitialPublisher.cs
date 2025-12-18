using System.Text.Json;
using RabbitMQ.Client;

namespace Payment.API.Messaging;

public class RabbitPaymentInitialPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly ILogger<RabbitPaymentInitialPublisher> _logger;

    public RabbitPaymentInitialPublisher(IConfiguration configuration, ILogger<RabbitPaymentInitialPublisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory()
        {
            HostName = configuration["Rabbit:HostName"],
            UserName = configuration["Rabbit:UserName"],
            Password = configuration["Rabbit:Password"],
            DispatchConsumersAsync = true
        };
        
        _exchange = configuration["Rabbit:Exchange"];
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
    }
    
    public Task PublishPaymentAsync(object data)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(data);
        var properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        _channel.BasicPublish(_exchange,"payment.created", properties, body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
