using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Consts;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create.MessageValidators;

public static class DeliverierBuildMessageBirthDate
{
    public static void Build(DeliverierCreateCommand data, StringBuilder message)
    {
        var paramName = nameof(data.BirthDate);

        if (data.BirthDate == default)
        {
            message.Append(paramName.AppendError());
            return;
        }

        var today = DateTime.UtcNow.Date;
        var age = today.Year - data.BirthDate.Year;

        if (data.BirthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        if (age < 18)
        {
            message.Append(DeliverierServiceConst.MESSAGE_INVALID_AGE);
        }
    }
}
