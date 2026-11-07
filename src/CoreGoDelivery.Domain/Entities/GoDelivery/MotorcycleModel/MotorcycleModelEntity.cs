using CoreGoDelivery.Domain.Entities.GoDelivery.Base;

namespace CoreGoDelivery.Domain.Entities.GoDelivery.MotorcycleModel;

public sealed class MotorcycleModelEntity : BaseEntity
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
}
