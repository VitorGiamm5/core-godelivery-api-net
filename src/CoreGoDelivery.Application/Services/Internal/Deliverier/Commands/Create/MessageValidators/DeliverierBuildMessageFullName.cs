using CoreGoDelivery.Application.Extensions;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;

public static class DeliverierBuildMessageFullName
{
    public static void Build(DeliverierCreateCommand data, StringBuilder message)
    {
        var paramName = nameof(data.FullName);

        if (string.IsNullOrWhiteSpace(data.FullName) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                data.FullName.Trim(),
                CoreGoDelivery.Domain.Consts.Regex.RegexCollectionPatterns.FULL_NAME_PATTERN,
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            message.Append(paramName.AppendError());
        }
    }
}
