using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create;

public sealed class RentalCreateCommand : IRequest<ActionResult>
{
    [JsonIgnore]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    [DefaultValue("Deliverier123")]
    public string DeliverierId { get; set; } = string.Empty;

    [DefaultValue("moto123")]
    public string MotorcycleId { get; set; } = string.Empty;

    [DefaultValue("2024-01-01T00:00:00Z")]
    public string? StartDate { get; set; }

    [DefaultValue("2024-01-07T23:59:59Z")]
    public string? EndDate { get; set; }

    [DefaultValue("2024-01-07T23:59:59Z")]
    public string? EstimatedReturnDate { get; set; }

    [DefaultValue(1)]
    public int PlanId { get; set; }

    [DefaultValue("2024-01-07T23:59:59Z")]
    public DateTime? ReturnedToBaseDate { get; set; }
}
