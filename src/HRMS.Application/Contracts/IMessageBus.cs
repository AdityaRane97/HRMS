namespace HRMS.Application.Contracts;

/// <summary>
/// Cloud-neutral message bus abstraction.
/// Supports Azure Service Bus, AWS SQS/SNS, Google Pub/Sub, and in-memory queues.
/// Used for asynchronous messaging, notifications, and event publishing.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publish a message to a topic.
    /// </summary>
    Task PublishAsync<T>(string topicName, T message, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Send a message to a queue.
    /// </summary>
    Task SendAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Subscribe to a topic (for receiver/processor).
    /// </summary>
    Task SubscribeAsync<T>(string topicName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class;
}
