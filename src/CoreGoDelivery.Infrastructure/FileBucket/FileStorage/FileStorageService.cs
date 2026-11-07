using CoreGoDelivery.Domain.MinIOFile;
using CoreGoDelivery.Infrastructure.FileBucket.MinIO;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace CoreGoDelivery.Infrastructure.FileBucket.FileStorage;

public class FileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public FileStorageService(IMinIOClientService minioClientService, IOptions<MinIOSettings> options)
    {
        _minioClient = minioClientService.GetClient();
        _bucketName = options.Value.BucketLicenseCnh;
    }

    public async Task UploadFileAsync(string fileName, Stream fileStream, string contentType)
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType));
    }

    public async Task DeleteFileAsync(string fileName)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName));
    }

    public async Task<FileResult> GetFileAsync(string fileName)
    {
        using var fileStream = new MemoryStream();

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(fileStream);
                fileStream.Position = 0;
            }));

        string base64Content = Convert.ToBase64String(fileStream.ToArray());

        return new FileResult
        {
            FileName = fileName,
            Base64Content = base64Content
        };
    }
}
