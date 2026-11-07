using CoreGoDelivery.Domain.Consts;
using CoreGoDelivery.Domain.Enums.LicenceDriverType;
using SkiaSharp;

namespace CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create.Common;

public static class ImageValidateExtensionFile
{
    private const int MaxImageDimension = 10_000;
    private const long MaxPixelCount = 12_000_000;

    public static (bool isValid, string errorMessage, FileExtensionValidEnum fileExtension) Build(byte[]? imageBytes)
    {
        if (imageBytes is null || imageBytes.Length < 10)
        {
            return InvalidImage();
        }

        try
        {
            var stream = new MemoryStream(imageBytes, writable: false);
            using var codec = SKCodec.Create(stream);
            if (codec is null || !IsValidDimensions(codec.Info.Width, codec.Info.Height))
            {
                return InvalidImage();
            }

            var fileExtension = codec.EncodedFormat switch
            {
                SKEncodedImageFormat.Png => FileExtensionValidEnum.png,
                SKEncodedImageFormat.Bmp => FileExtensionValidEnum.bmp,
                _ => FileExtensionValidEnum.none
            };

            if (fileExtension == FileExtensionValidEnum.none)
            {
                return InvalidImage();
            }

            using var decodedImage = SKBitmap.Decode(codec);
            return decodedImage is not null &&
                decodedImage.Width == codec.Info.Width &&
                decodedImage.Height == codec.Info.Height
                ? (true, string.Empty, fileExtension)
                : InvalidImage();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return InvalidImage();
        }
    }

    private static bool IsValidDimensions(int width, int height)
    {
        return width is > 0 and <= MaxImageDimension &&
            height is > 0 and <= MaxImageDimension &&
            (long)width * height <= MaxPixelCount;
    }

    private static (bool isValid, string errorMessage, FileExtensionValidEnum fileExtension) InvalidImage()
    {
        return (false, DeliverierServiceConst.MESSAGE_FORMAT_FILE_DEFAULT, FileExtensionValidEnum.none);
    }
}
