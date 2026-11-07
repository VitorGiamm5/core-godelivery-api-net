using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Delete;

public class MotorcycleDeleteCommand : IRequest<ActionResult>
{
    public MotorcycleDeleteCommand(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}
