using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create.Common;
using CoreGoDelivery.Domain.Enums.LicenceDriverType;
using Xunit;

namespace CoreGoDelivery.ApplicationTests.Services.Internal.LicenseDriver.Commands.Create.Common;

public class ImageValidateExtensionFileTests
{
    private const string OnePixelPng = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR4nGP4z8DwHwAFAAH/iZk9HQAAAABJRU5ErkJggg==";

    [Fact]
    public void Build_WhenPngIsValid_ReturnsPngExtension()
    {
        var imageBytes = Convert.FromBase64String(OnePixelPng);

        var result = ImageValidateExtensionFile.Build(imageBytes);

        Assert.True(result.isValid);
        Assert.Equal(FileExtensionValidEnum.png, result.fileExtension);
    }

    [Fact]
    public void Build_WhenBmpIsValid_ReturnsBmpExtension()
    {
        var result = ImageValidateExtensionFile.Build(CreateOnePixelBmp());

        Assert.True(result.isValid);
        Assert.Equal(FileExtensionValidEnum.bmp, result.fileExtension);
    }

    [Fact]
    public void Build_WhenBytesAreNotAnImage_ReturnsInvalid()
    {
        var result = ImageValidateExtensionFile.Build(new byte[10]);

        Assert.False(result.isValid);
        Assert.Equal(FileExtensionValidEnum.none, result.fileExtension);
        Assert.False(string.IsNullOrWhiteSpace(result.errorMessage));
    }

    [Fact]
    public void Build_WhenInputIsNull_ReturnsInvalid()
    {
        var result = ImageValidateExtensionFile.Build(null);

        Assert.False(result.isValid);
        Assert.Equal(FileExtensionValidEnum.none, result.fileExtension);
    }

    private static byte[] CreateOnePixelBmp()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write((ushort)0x4D42);
        writer.Write(58u);
        writer.Write(0u);
        writer.Write(54u);
        writer.Write(40u);
        writer.Write(1);
        writer.Write(1);
        writer.Write((ushort)1);
        writer.Write((ushort)24);
        writer.Write(0u);
        writer.Write(4u);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0u);
        writer.Write(0u);
        writer.Write(new byte[4]);

        return stream.ToArray();
    }
}
