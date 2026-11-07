using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create.Common;
using CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;
using CoreGoDelivery.Domain.Entities.GoDelivery.LicenceDriver;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Infrastructure.FileBucket.FileStorage;
using Moq;

namespace CoreGoDelivery.ApplicationTests.Services.Internal.LicenseDriver.Commands.Create;

public class LicenseDriverHandlerCompensationTests
{
    private const string LicenseNumber = "91827364501";
    private const string OnePixelPng = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR4nGP4z8DwHwAFAAH/iZk9HQAAAABJRU5ErkJggg==";

    [Fact]
    public async Task Handle_WhenDatabaseReferenceCannotBeSaved_RemovesUploadedObject()
    {
        var (handler, licenseRepository, fileStorage, command) = CreateHandler();
        licenseRepository
            .Setup(repository => repository.UpdateFileName(LicenseNumber, It.IsAny<string>()))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None).AsTask());

        Assert.Contains("InvalidOperationException", exception.Message);
        fileStorage.Verify(
            storage => storage.DeleteFileAsync($"CNH_{LicenseNumber}.png"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMinioUploadFails_RemovesPotentialPartialObject()
    {
        var (handler, licenseRepository, fileStorage, command) = CreateHandler();
        fileStorage
            .Setup(storage => storage.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new IOException("simulated storage write failure"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None).AsTask());

        Assert.Contains("IOException", exception.Message);
        licenseRepository.Verify(
            repository => repository.UpdateFileName(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        fileStorage.Verify(
            storage => storage.DeleteFileAsync($"CNH_{LicenseNumber}.png"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenObjectCompensationFails_ReturnsOnlySafeExceptionTypes()
    {
        var (handler, licenseRepository, fileStorage, command) = CreateHandler();
        licenseRepository
            .Setup(repository => repository.UpdateFileName(LicenseNumber, It.IsAny<string>()))
            .ReturnsAsync(false);
        fileStorage
            .Setup(storage => storage.DeleteFileAsync(It.IsAny<string>()))
            .ThrowsAsync(new IOException($"storage cleanup failed for CNH_{LicenseNumber}.png"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None).AsTask());

        Assert.Contains("InvalidOperationException", exception.Message);
        Assert.Contains("IOException", exception.Message);
        Assert.DoesNotContain(LicenseNumber, exception.Message);
    }

    private static (LicenseDriverHandler Handler, Mock<ILicenceDriverRepository> LicenseRepository, Mock<IFileStorageService> FileStorage, LicenseImageCommand Command) CreateHandler()
    {
        var license = new LicenceDriverEntity
        {
            Id = LicenseNumber,
            Type = Domain.Enums.LicenceDriverType.LicenseTypeEnum.A,
            ImageUrlReference = "PENDING",
            IssueDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ExpiryDate = new DateTime(2031, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var licenseRepository = new Mock<ILicenceDriverRepository>();
        licenseRepository.Setup(repository => repository.GetOneById(LicenseNumber)).ReturnsAsync(license);

        var deliverierRepository = new Mock<IDeliverierRepository>();
        deliverierRepository.Setup(repository => repository.GetOneByIdLicense(LicenseNumber)).ReturnsAsync(
            new DeliverierEntity
            {
                Id = "test-deliverier",
                LicenceDriverId = LicenseNumber,
                LicenceDriver = license,
            });

        var fileStorage = new Mock<IFileStorageService>();
        fileStorage
            .Setup(storage => storage.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var validator = new LicenseDriverValidator(licenseRepository.Object, deliverierRepository.Object);
        var handler = new LicenseDriverHandler(licenseRepository.Object, fileStorage.Object, validator);
        var command = new LicenseImageCommand
        {
            IdLicenseNumber = LicenseNumber,
            LicenseImageBase64 = Convert.FromBase64String(OnePixelPng),
        };

        return (handler, licenseRepository, fileStorage, command);
    }
}
