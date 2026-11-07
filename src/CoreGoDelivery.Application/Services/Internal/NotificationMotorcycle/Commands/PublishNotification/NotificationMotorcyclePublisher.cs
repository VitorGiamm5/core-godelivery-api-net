using CoreGoDelivery.Application.Services.External.NotificationMotorcycle.Queries.Consumer;
using CoreGoDelivery.Domain.NotificationMotorcycle;
using CoreGoDelivery.Domain.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CoreGoDelivery.Application.Services.Internal.NotificationMotorcycle.Commands.PublishNotification;

public sealed class NotificationMotorcyclePublisher : IAsyncDisposable
{
    private readonly RabbitMQConnectionService _connectionService;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<NotificationMotorcyclePublisher> _logger;
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private IChannel? _channel;

    public NotificationMotorcyclePublisher(
        RabbitMQConnectionService connectionService,
        IOptions<RabbitMQSettings> settings,
        ILogger<NotificationMotorcyclePublisher> logger)
    {
        _connectionService = connectionService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task PublishMotorcycleAsync(
        NotificationMotorcycleDto motorcycle,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(motorcycle);

        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is null || !_channel.IsOpen)
            {
                _channel = await _connectionService.Connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync(
                    queue: _settings.QueuesName.MotorcycleNotificationPublishQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);
            }

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(motorcycle));
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _settings.QueuesName.MotorcycleNotificationPublishQueue,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                "Failed to publish a motorcycle notification to queue {QueueName} ({ExceptionType})",
                _settings.QueuesName.MotorcycleNotificationPublishQueue,
                exception.GetType().Name);
            throw;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        _channelLock.Dispose();
    }
}
