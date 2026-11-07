using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create.Common;
using CoreGoDelivery.Domain.Entities.GoDelivery.Rental;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;
using System.Globalization;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Create;

public class RentalCreateHandler : IRequestHandler<RentalCreateCommand, ActionResult>
{
    public readonly IRentalPlanRepository _repositoryPlan;
    public readonly IRentalRepository _repositoryRental;

    public readonly RentalCreateValidate _validator;

    public RentalCreateHandler(
        IRentalPlanRepository repositoryPlan,
        IRentalRepository repositoryRental,
        RentalCreateValidate validator)
    {
        _repositoryPlan = repositoryPlan;
        _repositoryRental = repositoryRental;
        _validator = validator;
    }

    public async ValueTask<ActionResult> Handle(RentalCreateCommand request, CancellationToken cancellationToken)
    {
        request.Id = Guid.NewGuid().ToString();

        var apiReponse = new ActionResult();

        apiReponse.SetError(await _validator.BuilderCreateValidator(request));

        if (apiReponse.HasError())
        {
            return apiReponse;
        }

        var plan = await _repositoryPlan.GetById(request.PlanId);

        var startDateUtc = ResolveStartDateUtc(request.StartDate);
        RentalEntity calculatedDates;
        try
        {
            calculatedDates = RentalCalculateDatesByPlan.Calculate(plan!, startDateUtc);
        }
        catch (ArgumentOutOfRangeException)
        {
            apiReponse.SetError("Invalid rental dates: plan duration exceeds the supported date range; ");
            return apiReponse;
        }

        var rental = RentalCreateMappers.MapCreateToEntity(request, calculatedDates);

        var isSuccess = await _repositoryRental.Create(rental);

        if (!isSuccess)
        {
            apiReponse.SetError(nameof(_repositoryRental.Create).AppendError(AdditionalMessageEnum.CreateFail));
        }

        apiReponse.SetData(rental);

        return apiReponse;
    }

    private static DateTime ResolveStartDateUtc(string? startDate)
    {
        if (string.IsNullOrWhiteSpace(startDate))
        {
            return DateTime.UtcNow;
        }

        var parsedDate = DateTime.Parse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        return parsedDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc)
            : parsedDate.ToUniversalTime();
    }
}
