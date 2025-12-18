namespace MicroServiceCategory.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object data);
}