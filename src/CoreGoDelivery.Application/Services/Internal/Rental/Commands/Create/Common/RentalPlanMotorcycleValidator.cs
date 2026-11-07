using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Entities.GoDelivery.RentalPlan;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using System.Text;
using System.Globalization;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create.Common;

public static class RentalPlanMotorcycleValidator
{
    public static StringBuilder? Validade(RentalCreateCommand data, RentalPlanEntity plan)
    {
        var message = new StringBuilder();

        var hasStartDate = !string.IsNullOrWhiteSpace(data.StartDate);
        var hasEndDate = !string.IsNullOrWhiteSpace(data.EndDate);
        var hasEstimatedReturnDate = !string.IsNullOrWhiteSpace(data.EstimatedReturnDate);

        var hasValidStartDate = TryParseDate(nameof(data.StartDate), data.StartDate, message, out var startDate);
        var hasValidEndDate = TryParseDate(nameof(data.EndDate), data.EndDate, message, out var endDate);
        var hasValidEstimatedReturnDate = TryParseDate(nameof(data.EstimatedReturnDate), data.EstimatedReturnDate, message, out var estimatedReturnDate);

        if (plan.DaysQuantity <= 0)
        {
            message.Append("Invalid rental plan: duration must be greater than zero; ");
        }

        if ((hasEndDate || hasEstimatedReturnDate) && !hasStartDate)
        {
            message.Append("Invalid rental dates: StartDate is required when EndDate or EstimatedReturnDate is supplied; ");
        }

        if (data.ReturnedToBaseDate.HasValue)
        {
            message.Append("Invalid rental dates: ReturnedToBaseDate must be recorded through the return-to-base operation; ");
        }

        if (hasValidStartDate && plan.DaysQuantity > 0)
        {
            try
            {
                var expectedReturnDate = startDate.AddDays(plan.DaysQuantity);
                if (hasValidEndDate && endDate != expectedReturnDate)
                {
                    message.Append($"Invalid field: {nameof(data.EndDate)} must equal StartDate plus {plan.DaysQuantity} plan days; ");
                }

                if (hasValidEstimatedReturnDate && estimatedReturnDate != expectedReturnDate)
                {
                    message.Append($"Invalid field: {nameof(data.EstimatedReturnDate)} must equal StartDate plus {plan.DaysQuantity} plan days; ");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                message.Append("Invalid rental dates: StartDate and plan duration exceed the supported date range; ");
            }
        }

        return message;
    }

    private static bool TryParseDate(string fieldName, string? value, StringBuilder message, out DateTimeOffset parsedDate)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsedDate = default;
            return false;
        }

        if (DateTimeOffset.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out parsedDate))
        {
            return true;
        }

        message.Append(fieldName.AppendError(AdditionalMessageEnum.InvalidDate));
        return false;
    }
}
