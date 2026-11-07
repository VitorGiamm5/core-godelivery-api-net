using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Consts;
using CoreGoDelivery.Domain.Entities.GoDelivery.Rental;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update.Common;

public static class RentalCalculatePenalty
{
    public static object Calculate(DateTime returnedToBaseDate, RentalEntity rental)
    {
        TimeSpan delay = returnedToBaseDate - rental.EstimatedReturnDate;

        if (delay == TimeSpan.Zero)
        {
            return CalculateCharge(
                returnedToBaseDate,
                rental,
                RentalServiceConst.MESSAGE_RETURNED_TO_BASE_SUCCESS,
                penaltyValue: 0.0);
        }

        if (delay > TimeSpan.Zero)
        {
            var startedDaysLate = (int)Math.Ceiling(delay.TotalDays);
            var valueToPay = RentalExpiredDateToReturn.Calculate(startedDaysLate);

            return CalculateCharge(
                returnedToBaseDate,
                rental,
                RentalServiceConst.MESSAGE_RETURNED_AFTER,
                valueToPay);
        }

        if (delay < TimeSpan.Zero)
        {
            var fullDaysEarly = delay.Days;
            var valueToPay = RentalReturnerBeforeExpected.Calculate(rental, fullDaysEarly);

            return CalculateCharge(
                returnedToBaseDate,
                rental,
                RentalServiceConst.MESSAGE_RETURNED_BEFORE,
                valueToPay);
        }

        throw new InvalidOperationException("The return penalty could not be calculated.");
    }

    private static object CalculateCharge(
        DateTime returnedToBaseDate,
        RentalEntity rental,
        string message,
        double penaltyValue)
    {
        var planDays = rental.RentalPlan!.DaysQuantity;
        var chargeableDays = returnedToBaseDate < rental.EstimatedReturnDate
            ? planDays - (int)Math.Ceiling((rental.EstimatedReturnDate - returnedToBaseDate).TotalDays)
            : planDays + (returnedToBaseDate - rental.EstimatedReturnDate).Days;
        chargeableDays = Math.Max(0, chargeableDays);
        var rentalAmount = rental.RentalPlan!.DayliCost * chargeableDays;

        return new
        {
            Message = message,
            Value = penaltyValue,
            ChargeableDays = chargeableDays,
            RentalAmount = rentalAmount,
            TotalCharge = rentalAmount + penaltyValue
        };
    }
}
