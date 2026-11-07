using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;

public static class DeliverierBuildMessageLicenseDates
{
    public static void Build(DeliverierCreateCommand data, StringBuilder message)
    {
        var today = DateTime.UtcNow.Date;

        if (data.IssueDate.Date > today)
        {
            message.Append(nameof(data.IssueDate).AppendError(AdditionalMessageEnum.InvalidDate));
        }

        if (data.ExpiryDate.Date < today)
        {
            message.Append(nameof(data.ExpiryDate).AppendError(AdditionalMessageEnum.InvalidDate));
        }

        if (data.ExpiryDate.Date <= data.IssueDate.Date)
        {
            message.Append(nameof(data.ExpiryDate).AppendError(AdditionalMessageEnum.InvalidDate));
        }
    }
}
