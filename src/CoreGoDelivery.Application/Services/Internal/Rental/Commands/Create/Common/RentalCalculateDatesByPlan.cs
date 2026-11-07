using CoreGoDelivery.Domain.Entities.GoDelivery.Rental;
using CoreGoDelivery.Domain.Entities.GoDelivery.RentalPlan;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create.Common;

public static class RentalCalculateDatesByPlan
{
    public static RentalEntity Calculate(RentalPlanEntity plan)
    {
        return Calculate(plan, DateTime.UtcNow);
    }

    public static RentalEntity Calculate(RentalPlanEntity plan, DateTime startDateUtc)
    {
        var returnDate = startDateUtc.AddDays(plan.DaysQuantity);

        return new RentalEntity
        {
            StartDate = startDateUtc,
            EndDate = returnDate,
            EstimatedReturnDate = returnDate
        };
    }
}
