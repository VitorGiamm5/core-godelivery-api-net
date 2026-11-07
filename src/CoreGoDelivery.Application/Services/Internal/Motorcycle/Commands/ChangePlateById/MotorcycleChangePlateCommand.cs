using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.ChangePlateById;

public class MotorcycleChangePlateCommand : IRequest<ActionResult>
{
    [JsonIgnore]
    public string? Id { get; set; }

    public string? Plate { get; set; }
}
