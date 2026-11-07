using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;
using CoreGoDelivery.Application.Services.Internal.NotificationMotorcycle.Commands.PublishNotification;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Create;

public class MotorcycleCreateHandler : IRequestHandler<MotorcycleCreateCommand, ActionResult>
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;

    private readonly MotorcycleCreateValidator _validator;
    private readonly NotificationMotorcyclePublisher _notification;

    public MotorcycleCreateHandler(
        IMotorcycleRepository repositoryMotorcycle,
        MotorcycleCreateValidator validator,
        NotificationMotorcyclePublisher notification)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
        _validator = validator;
        _notification = notification;
    }

    public async ValueTask<ActionResult> Handle(MotorcycleCreateCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();

        request.Id = Ulid.NewUlid().ToString();
        request.Plate = request.Plate.RemoveCharactersToUpper();

        apiReponse.SetError(await _validator.Validate(request));

        if (apiReponse.HasError())
        {
            return apiReponse;
        }

        var motorcycle = MotorcycleServiceMappers.MapCreateToEntity(request);

        var isSuccess = await _repositoryMotorcycle.Create(motorcycle);

        if (!isSuccess)
        {
            apiReponse.SetError(nameof(_repositoryMotorcycle.Create).AppendError(AdditionalMessageEnum.CreateFail));

            return apiReponse;
        }

        if (motorcycle.YearManufacture == DateTime.Today.Year)
        {
            var notification = MotorcycleServiceMappers.MapNotificationDto(motorcycle);

            await _notification.PublishMotorcycleAsync(notification, cancellationToken);
        }

        apiReponse.SetData(motorcycle);

        return apiReponse;
    }
}
