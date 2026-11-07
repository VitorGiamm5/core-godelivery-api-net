using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create.Common;
using CoreGoDelivery.Domain.Entities.GoDelivery.RentalPlan;
using Assert = Xunit.Assert;

namespace CoreGoDelivery.ApplicationTests.Services.Internal.Rental.Commands.Create.Common;

public class CalculateDatesByPlanTests
{
    [Fact]
    public void Calculate_WithKnownUtcStartDate_ReturnsCorrectRentalDates()
    {
        var startDate = new DateTime(2026, 11, 4, 12, 30, 0, DateTimeKind.Utc);
        var plan = new RentalPlanEntity { DaysQuantity = 5 };
        var expectedReturnDate = startDate.AddDays(plan.DaysQuantity);

        var result = RentalCalculateDatesByPlan.Calculate(plan, startDate);

        Assert.NotNull(result);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(expectedReturnDate, result.EndDate);
        Assert.Equal(expectedReturnDate, result.EstimatedReturnDate);
    }
}
