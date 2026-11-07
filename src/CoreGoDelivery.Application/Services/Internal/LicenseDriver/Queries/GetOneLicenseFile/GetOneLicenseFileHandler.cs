using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using CoreGoDelivery.Infrastructure.FileBucket.FileStorage;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.LicenseDriver.Queries.GetOneLicenseFile;

public class GetOneLicenseFileHandler : IRequestHandler<GetOneLicenseFileCommand, ActionResult>
{
    public readonly IDeliverierRepository _repositoryDelivarier;
    public readonly ILicenceDriverRepository _repositoryLicence;
    public readonly IFileStorageService _fileService;

    public GetOneLicenseFileHandler(
        IDeliverierRepository repositoryDelivarier,
        ILicenceDriverRepository repositoryLicence,
        IFileStorageService fileService)
    {
        _repositoryDelivarier = repositoryDelivarier;
        _repositoryLicence = repositoryLicence;
        _fileService = fileService;
    }

    public async ValueTask<ActionResult> Handle(GetOneLicenseFileCommand request, CancellationToken cancellationToken)
    {
        var apiResponse = new ActionResult();

        #region validation
        var deliverier = await _repositoryDelivarier.GetOneByIdLicense(request.Id);

        if (deliverier == null)
        {
            apiResponse.SetError("deliverier".AppendError(AdditionalMessageEnum.NotFound));
            return apiResponse;
        }

        var license = await _repositoryLicence.GetOneById(request.Id);

        if (license == null)
        {
            apiResponse.SetError("license".AppendError(AdditionalMessageEnum.NotFound));
            return apiResponse;
        }

        if (string.IsNullOrEmpty(license.ImageUrlReference) || license.IsPendingImage())
        {
            apiResponse.SetError("ImageUrlReference".AppendError(AdditionalMessageEnum.NotFound));
            return apiResponse;
        }

        #endregion

        var base64File = await _fileService.GetFileAsync(license.ImageUrlReference);

        if (base64File == null)
        {
            apiResponse.SetError("base64File".AppendError(AdditionalMessageEnum.NotFound));
            return apiResponse;
        }

        apiResponse.SetData(new { licenseImageBase64 = base64File });

        return apiResponse;
    }
}
