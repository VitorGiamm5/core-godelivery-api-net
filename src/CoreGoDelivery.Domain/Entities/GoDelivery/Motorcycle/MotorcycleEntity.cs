using CoreGoDelivery.Domain.Entities.GoDelivery.Base;
using CoreGoDelivery.Domain.Entities.GoDelivery.MotorcycleModel;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;

public class MotorcycleEntity : BaseEntity
{
    public string Id { get; set; } = null!;
    public int YearManufacture { get; set; }
    public string PlateNormalized { get; set; } = null!;

    #region relationships

    [JsonIgnore]
    public string MotorcycleModelId { get; set; } = null!;

    [JsonIgnore]
    public MotorcycleModelEntity? MotorcycleModel { get; set; }

    #endregion
}
