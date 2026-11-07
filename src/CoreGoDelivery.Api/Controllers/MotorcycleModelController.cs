using CoreGoDelivery.Api.Controllers.Base;
using CoreGoDelivery.Application.Services.Internal.MotorcycleModel.Queries.List;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CoreGoDelivery.Api.Controllers;

[Route("api/motorcycle-models")]
[ApiController]
public class MotorcycleModelController(IMediator _mediator, ILogger<BaseApiController> logger) : BaseApiController(logger)
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] MotorcycleModelQueryCommand request)
    {
        try
        {
            var result = await _mediator.Send(request);

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }
}
