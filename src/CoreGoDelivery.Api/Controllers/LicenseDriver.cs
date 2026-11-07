using CoreGoDelivery.Api.Controllers.Base;
using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.LicenseDriver.Queries.GetOneLicenseFile;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CoreGoDelivery.Api.Controllers;

[Route("api/license-driver")]
[ApiController]
public class LicenseDriver(IMediator _mediator, ILogger<BaseApiController> logger) : BaseApiController(logger)
{

    [HttpPost("{id}/upload-cnh")]
    public async Task<IActionResult> Upload(string id, [FromBody] LicenseImageCommand request)
    {
        try
        {
            IdParamValidator(id);

            request.IdLicenseNumber = id;

            var apiReponse = await _mediator.Send(request);

            return Response(apiReponse);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }

    [HttpGet("{id}/license-File")]
    public async Task<IActionResult> GetOneLicenseFile(string id)
    {
        try
        {
            IdParamValidator(id);

            var result = await _mediator.Send(new GetOneLicenseFileCommand(id));

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }
}
