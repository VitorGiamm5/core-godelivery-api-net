using CoreGoDelivery.Api.Controllers.Base;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.ChangePlateById;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Delete;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.GetOne;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.List;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CoreGoDelivery.Api.Controllers;

[Route("api/motorcycles")]
[ApiController]
public class MotorcycleController(IMediator _mediator, ILogger<BaseApiController> logger) : BaseApiController(logger)
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] MotorcycleListQueryCommand request)
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
        try
        {
            IdParamValidator(id);

            var result = await _mediator.Send(new MotorcycleGetOneQueryCommand(id));

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MotorcycleCreateCommand request)
    {
        try
        {
            request.Id = IdBuild(request.Id);

            var result = await _mediator.Send(request);

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }

    [HttpPut("{id}/plate")]
    public async Task<IActionResult> Put(string id, [FromBody] MotorcycleChangePlateCommand request)
    {
        try
        {
            IdParamValidator(id);

            request.Id = id;

            var result = await _mediator.Send(request);

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            IdParamValidator(id);

            var result = await _mediator.Send(new MotorcycleDeleteCommand(id));

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }
}
