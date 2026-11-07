using CoreGoDelivery.Domain.Entities.GoDelivery.NotificationMotorcycle;
using CoreGoDelivery.Domain.NotificationMotorcycle;
using CoreGoDelivery.Domain.RabbitMQ;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace CoreGoDelivery.Application.Services.External.NotificationMotorcycle.Queries.Consumer;

public sealed class NotificationMotorcycleConsumer : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly RabbitMQConnectionService _connectionService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<NotificationMotorcycleConsumer> _logger;
    private IChannel? _channel;

    public NotificationMotorcycleConsumer(
        RabbitMQConnectionService connectionService,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<RabbitMQSettings> settings,
        ILogger<NotificationMotorcycleConsumer> logger)
    {
        _connectionService = connectionService;
        _serviceScopeFactory = serviceScopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _channel = await _connectionService.Connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(
                queue: _settings.QueuesName.MotorcycleNotificationConsumerQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessageAsync;

            await _channel.BasicConsumeAsync(
                queue: _settings.QueuesName.MotorcycleNotificationConsumerQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation(
                "RabbitMQ consumer started for queue {QueueName}",
                _settings.QueuesName.MotorcycleNotificationConsumerQueue);

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            if (_channel is not null)
            {
                if (_channel.IsOpen)
                {
                    await _channel.CloseAsync();
                }

                await _channel.DisposeAsync();
                _channel = null;
            }
        }
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var channel = _channel;
        if (channel is null)
        {
            return;
        }

        NotificationMotorcycleDto? notification;
        try
        {
            notification = JsonSerializer.Deserialize<NotificationMotorcycleDto>(
                eventArgs.Body.ToArray(),
                SerializerOptions);

            if (notification is null)
            {
                throw new JsonException("The message payload was empty.");
            }
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                "Discarding an invalid message from queue {QueueName}: {ExceptionType}",
                _settings.QueuesName.MotorcycleNotificationConsumerQueue,
                exception.GetType().Name);
            await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            return;
        }

        var entityNotification = new NotificationMotorcycleEntity
        {
            Id = notification.Id,
            IdMotorcycle = notification.MotorcycleId,
            YearManufacture = notification.YearManufacture,
            DateCreated = notification.DateCreated,
            CreatedBy = notification.CreatedBy
        };

        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationMotorcycleRepository>();
            await notificationRepository.CreateAsync(entityNotification);
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                "Failed to persist a notification from queue {QueueName}; requeuing message ({ExceptionType})",
                _settings.QueuesName.MotorcycleNotificationConsumerQueue,
                exception.GetType().Name);

            if (channel.IsOpen)
            {
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }
    }
}
