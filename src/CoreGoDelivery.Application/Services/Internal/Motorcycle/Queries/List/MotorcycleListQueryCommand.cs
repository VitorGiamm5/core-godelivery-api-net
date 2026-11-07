using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.ComponentModel;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.List;

public class MotorcycleListQueryCommand : MotorcycleListQuerybase, IRequest<ActionResult>
{
    [DefaultValue("abc-1234")]
    public string? Plate { get; set; }
}
