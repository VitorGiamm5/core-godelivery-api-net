using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update.Common;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Rental.Commands.Update;

public class RentalReturnedToBaseDateHandler : IRequestHandler<RentalReturnedToBaseDateCommand, ActionResult>
{
    public readonly IRentalRepository _repositoryRental;

    public readonly RentalReturnedToBaseValidator _validator;

    public RentalReturnedToBaseDateHandler(
        IRentalRepository repositoryRental,
        RentalReturnedToBaseValidator validator)
    {
        _repositoryRental = repositoryRental;
        _validator = validator;
    }

    public async ValueTask<ActionResult> Handle(RentalReturnedToBaseDateCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();

        apiReponse.SetError(await _validator.BuilderUpdateValidator(request));

        if (apiReponse.HasError())
        {
            return apiReponse;
        }

        var rental = await _repositoryRental.GetByIdAsync(request.Id!);

        if (rental == null)
        {
            apiReponse.SetError(nameof(rental).AppendError(AdditionalMessageEnum.NotFound));
            return apiReponse;
        }

        var returnedDate = request!.ReturnedToBaseDate!.Value;

        var calculatedResult = RentalCalculatePenalty.Calculate(returnedDate, rental);

        apiReponse.SetData(calculatedResult);

        var successUpdate = await _repositoryRental.UpdateReturnedToBaseDate(request.Id!, returnedDate);

        if (!successUpdate)
        {
            apiReponse.SetError(nameof(request.ReturnedToBaseDate).AppendError(AdditionalMessageEnum.UpdateFail));

            return apiReponse;
        }

        return apiReponse;
    }
}
