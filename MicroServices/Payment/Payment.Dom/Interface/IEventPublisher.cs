namespace Payment.Dom.Interface;

public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object eventData);
}