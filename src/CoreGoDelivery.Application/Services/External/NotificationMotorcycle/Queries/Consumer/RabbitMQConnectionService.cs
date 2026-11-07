using CoreGoDelivery.Domain.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CoreGoDelivery.Application.Services.External.NotificationMotorcycle.Queries.Consumer;

public sealed class RabbitMQConnectionService : IHostedService, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnectionService> _logger;
    private IConnection? _connection;

    public RabbitMQConnectionService(
        IConnectionFactory connectionFactory,
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQConnectionService> logger)
    {
        _connectionFactory = connectionFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public IConnection Connection => _connection
        ?? throw new InvalidOperationException("The RabbitMQ connection has not been started.");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}", _settings.Host, _settings.Port);
                return;
            }
            catch (Exception exception) when (attempt < _settings.ConnectionRetryCount)
            {
                var delay = TimeSpan.FromSeconds(_settings.RetryInitialDelaySeconds * Math.Pow(2, attempt - 1));
                _logger.LogWarning(
                    "RabbitMQ connection attempt {Attempt} failed with {ExceptionType}; retrying in {DelaySeconds} seconds",
                    attempt,
                    exception.GetType().Name,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            await _connection.CloseAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
