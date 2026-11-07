using CoreGoDelivery.Application.Services.External.NotificationMotorcycle.Queries.Consumer;
using CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;
using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.ChangePlateById;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Delete;
using CoreGoDelivery.Application.Services.Internal.NotificationMotorcycle.Commands.PublishNotification;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create.MessageValidators;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update;
using CoreGoDelivery.Domain.RabbitMQ;
using CoreGoDelivery.Infrastructure;
using CoreGoDelivery.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CoreGoDelivery.Application;

public static class SetupApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.BuildMessageValidator();

        services.AddOptions<RabbitMQSettings>()
            .Bind(configuration.GetSection("RabbitMQ"))
            .Validate(settings =>
                !string.IsNullOrWhiteSpace(settings.Host) &&
                !string.IsNullOrWhiteSpace(settings.Username) &&
                !string.IsNullOrWhiteSpace(settings.Password) &&
                settings.Port is > 0 and <= 65535 &&
                settings.ConnectionRetryCount > 0 &&
                settings.RetryInitialDelaySeconds > 0 &&
                !string.IsNullOrWhiteSpace(settings.QueuesName.MotorcycleNotificationPublishQueue) &&
                !string.IsNullOrWhiteSpace(settings.QueuesName.MotorcycleNotificationConsumerQueue),
                "RabbitMQ settings are incomplete or invalid.")
            .ValidateOnStart();

        services.TryAddSingleton<IConnectionFactory>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            return new ConnectionFactory
            {
                HostName = settings.Host,
                UserName = settings.Username,
                Password = settings.Password,
                Port = settings.Port,
                AutomaticRecoveryEnabled = true,
                ClientProvidedName = "CoreGoDelivery"
            };
        });

        services.AddSingleton<RabbitMQConnectionService>();
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMQConnectionService>());

        services.TryAddSingleton<NotificationMotorcyclePublisher>();
        services.AddHostedService<NotificationMotorcycleConsumer>();

        return services;
    }

    private static IServiceCollection BuildMessageValidator(this IServiceCollection services)
    {
        AddRentalServices(services);
        AddDeliverierServices(services);
        AddMotorcycleServices(services);

        return services;
    }

    private static void AddMotorcycleServices(IServiceCollection services)
    {
        services.TryAddScoped<MotorcycleChangePlateValidator>();
        services.TryAddScoped<MotorcycleCreateValidator>();
        services.TryAddScoped<MotorcycleDeleteValidator>();
    }

    private static void AddDeliverierServices(IServiceCollection services)
    {
        services.TryAddScoped<DeliverierBuildMessageCnh>();
        services.TryAddScoped<DeliverierBuildMessageCnpj>();
        services.TryAddScoped<DeliverierBuildMessageDeliverierCreate>();
        services.TryAddScoped<DeliverierCreateValidator>();
        services.TryAddScoped<LicenseDriverValidator>();
    }

    private static void AddRentalServices(IServiceCollection services)
    {
        services.TryAddScoped<RentalBuildMessageDeliverierId>();
        services.TryAddScoped<RentalBuildMessageMotorcycleId>();
        services.TryAddScoped<RentalBuildMessagePlanId>();
        services.TryAddScoped<RentalCreateValidate>();
        services.TryAddScoped<RentalReturnedToBaseValidator>();
    }
}
