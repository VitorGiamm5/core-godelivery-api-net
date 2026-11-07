using CoreGoDelivery.Domain.MinIOFile;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace CoreGoDelivery.Infrastructure.FileBucket.MinIO;

public class MinIOClientService : IMinIOClientService
{
    private readonly IMinioClient _minioClient;

    public MinIOClientService(IOptions<MinIOSettings> options)
    {
        var minioSettings = options.Value;
        _minioClient = new MinioClient()
            .WithEndpoint($"{minioSettings.Endpoint}:{minioSettings.Port}")
            .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
            .WithSSL(minioSettings.UseSSL)
            .Build();
    }

    public IMinioClient GetClient() => _minioClient;

    public async Task EnsureBucketExistsAsync(string bucketName)
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
    }
}
