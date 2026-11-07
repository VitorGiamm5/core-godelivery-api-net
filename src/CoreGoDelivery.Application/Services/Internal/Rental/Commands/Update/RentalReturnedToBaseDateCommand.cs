using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update;

public class RentalReturnedToBaseDateCommand : IRequest<ActionResult>
{
    [JsonIgnore]
    public string? Id { get; set; }

    [DefaultValue("2024-01-07")]
    public DateTime? ReturnedToBaseDate { get; set; }
}
