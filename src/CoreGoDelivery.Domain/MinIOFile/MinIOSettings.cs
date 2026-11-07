namespace CoreGoDelivery.Domain.MinIOFile;

public sealed class MinIOSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public int Port { get; set; }
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; }
    public string BucketLicenseCnh { get; set; } = string.Empty;
}
