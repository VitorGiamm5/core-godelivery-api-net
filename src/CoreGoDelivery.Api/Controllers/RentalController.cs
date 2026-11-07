using CoreGoDelivery.Api.Controllers.Base;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update;
using CoreGoDelivery.Application.Services.Internal.Rental.Queries.GetOne;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CoreGoDelivery.Api.Controllers;

[Route("api/rentals")]
[ApiController]
public class RentalController(IMediator _mediator, ILogger<BaseApiController> logger) : BaseApiController(logger)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(string? id)
    {
        try
        {
            IdParamValidator(id);

            var request = new RentalGetOneCommand(id!);

            var result = await _mediator.Send(request);

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RentalCreateCommand request)
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

    [HttpPut("{id}/return-to-base")]
    public async Task<IActionResult> UpdateReturnedToBaseDate(string? id, [FromBody] RentalReturnedToBaseDateCommand request)
    {
        try
        {
            IdParamValidator(id);

            request.Id = id!;

            var result = await _mediator.Send(request);

            return Response(result);
        }
        catch (Exception ex)
        {
            return ResponseError(ex);
        }
    }
}
