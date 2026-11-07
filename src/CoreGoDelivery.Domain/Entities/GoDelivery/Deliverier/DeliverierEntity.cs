using CoreGoDelivery.Domain.Entities.GoDelivery.Base;
using CoreGoDelivery.Domain.Entities.GoDelivery.LicenceDriver;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;

public sealed class DeliverierEntity : BaseEntity
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public DateTime BirthDate { get; set; }

    #region relationships

    [JsonIgnore]
    public string LicenceDriverId { get; set; } = null!;

    [JsonIgnore]
    public LicenceDriverEntity LicenceDriver { get; set; } = null!;

    #endregion
}
