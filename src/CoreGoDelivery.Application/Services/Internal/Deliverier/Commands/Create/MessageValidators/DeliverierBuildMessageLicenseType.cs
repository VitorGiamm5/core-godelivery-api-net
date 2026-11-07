using CoreGoDelivery.Application.Extensions;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;

public static class DeliverierBuildMessageLicenseType
{
    public static void Build(DeliverierCreateCommand data, StringBuilder message)
    {
        var paramName = nameof(data.LicenseType);

        var licenseType = data.LicenseType?.Trim();
        if (!string.Equals(licenseType, "A", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(licenseType, "AB", StringComparison.OrdinalIgnoreCase))
        {
            message.Append(paramName.AppendError());
        }
    }
}
