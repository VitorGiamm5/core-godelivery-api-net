using CoreGoDelivery.Domain;
using CoreGoDelivery.Domain.MinIOFile;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Infrastructure.Database;
using CoreGoDelivery.Infrastructure.FileBucket.FileStorage;
using CoreGoDelivery.Infrastructure.FileBucket.MinIO;
using CoreGoDelivery.Infrastructure.Repositories.GoDelivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CoreGoDelivery.Infrastructure;

public static class SetupInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDomain(configuration);

        AddRepositories(services);

        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options.UseNpgsql().AddInfrastructure(configuration);
        });

        services.AddOptions<MinIOSettings>()
            .Bind(configuration.GetSection("MinIOSettings"))
            .Validate(settings =>
                !string.IsNullOrWhiteSpace(settings.Endpoint) &&
                settings.Port is > 0 and <= 65535 &&
                !string.IsNullOrWhiteSpace(settings.AccessKey) &&
                !string.IsNullOrWhiteSpace(settings.SecretKey) &&
                !string.IsNullOrWhiteSpace(settings.BucketLicenseCnh),
                "MinIO settings are incomplete or invalid.")
            .ValidateOnStart();

        services.TryAddSingleton<IMinIOClientService, MinIOClientService>();
        services.TryAddScoped<IFileStorageService, FileStorageService>();

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.TryAddScoped<IDeliverierRepository, DeliverierRepository>();
        services.TryAddScoped<ILicenceDriverRepository, LicenceDriverRepository>();
        services.TryAddScoped<IMotorcycleModelRepository, MotorcycleModelRepository>();
        services.TryAddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.TryAddScoped<IRentalPlanRepository, RentalPlanRepository>();
        services.TryAddScoped<IRentalRepository, RentalRepository>();
        services.TryAddScoped<INotificationMotorcycleRepository, NotificationMotorcycleRepository>();
    }
}
