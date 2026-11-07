using CoreGoDelivery.Api.Controllers.Base;
using CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CoreGoDelivery.Api.Controllers;

[Route("api/deliveriers")]
[ApiController]
public class DeliveriersController(IMediator _mediator, ILogger<BaseApiController> logger) : BaseApiController(logger)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeliverierCreateCommand request)
    {
        try
        {
            var apiReponse = await _mediator.Send(request);

            return Response(apiReponse);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }
}
