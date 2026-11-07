using CoreGoDelivery.Domain.Entities.GoDelivery.Base;

namespace CoreGoDelivery.Domain.NotificationMotorcycle;

public sealed class NotificationMotorcycleDto : BaseEntity
{
    public required string Id { get; set; }
    public required string ModelMotorcycleId { get; set; }
    public required string PlateNormalized { get; set; }
    public required string MotorcycleId { get; set; }
    public int YearManufacture { get; set; }
}
