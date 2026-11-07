namespace CoreGoDelivery.Domain.RabbitMQ;

public class RabbitMQSettings
{
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
    public int ConnectionRetryCount { get; set; } = 5;
    public int RetryInitialDelaySeconds { get; set; } = 2;
    public RabbitMQQueueSettings QueuesName { get; set; } = new();
}

public sealed class RabbitMQQueueSettings
{
    public string MotorcycleNotificationPublishQueue { get; set; } = string.Empty;
    public string MotorcycleNotificationConsumerQueue { get; set; } = string.Empty;
}
