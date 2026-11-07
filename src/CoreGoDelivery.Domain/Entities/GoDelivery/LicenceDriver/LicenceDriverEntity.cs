using CoreGoDelivery.Domain.Consts;
using CoreGoDelivery.Domain.Entities.GoDelivery.Base;
using CoreGoDelivery.Domain.Enums.LicenceDriverType;

namespace CoreGoDelivery.Domain.Entities.GoDelivery.LicenceDriver;

public sealed class LicenceDriverEntity : BaseEntity
{
    /// <summary>
    /// Use License Number as Id
    /// </summary>
    public string Id { get; set; } = null!;
    public LicenseTypeEnum Type { get; set; }
    public string ImageUrlReference { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? DateUpdated { get; set; }

    public bool IsPendingImage()
    {
        return string.Equals(ImageUrlReference, LicenseImageConst.PENDING_IMAGE_LICENSE, StringComparison.OrdinalIgnoreCase);
    }
}
