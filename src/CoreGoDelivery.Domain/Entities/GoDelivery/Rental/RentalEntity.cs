using CoreGoDelivery.Domain.Entities.GoDelivery.Base;
using CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;
using CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;
using CoreGoDelivery.Domain.Entities.GoDelivery.RentalPlan;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Domain.Entities.GoDelivery.Rental;

public sealed class RentalEntity : BaseEntity
{
    public string Id { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime EstimatedReturnDate { get; set; }
    public DateTime? ReturnedToBaseDate { get; set; }

    #region Relationships

    [JsonIgnore]
    public string? DeliverierId { get; set; }

    [JsonIgnore]
    public DeliverierEntity? Deliverier { get; set; }

    [JsonIgnore]
    public string? MotorcycleId { get; set; }

    [JsonIgnore]
    public MotorcycleEntity? Motorcycle { get; set; }

    [JsonIgnore]
    public int RentalPlanId { get; set; }

    [JsonIgnore]
    public RentalPlanEntity? RentalPlan { get; set; }

    #endregion
}
