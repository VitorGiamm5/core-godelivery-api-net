using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Deliverier.Commands.Create;

public class DeliverierCreateHandler : IRequestHandler<DeliverierCreateCommand, ActionResult>
{
    public readonly IDeliverierRepository _repositoryDeliverier;
    public readonly DeliverierCreateValidator _validator;

    public DeliverierCreateHandler(
        IDeliverierRepository repositoryDeliverier,
        DeliverierCreateValidator validator)
    {
        _repositoryDeliverier = repositoryDeliverier;
        _validator = validator;
    }

    public async ValueTask<ActionResult> Handle(DeliverierCreateCommand request, CancellationToken cancellationToken)
    {
        request.Id = Guid.NewGuid().ToString();

        var apiReponse = new ActionResult();

        apiReponse.SetError(await _validator.Validate(request));

        if (apiReponse.HasError())
        {
            return apiReponse;
        }

        var deliverier = DeliverierCreateMappers.MapCreateToEntity(request);

        var resultCreate = await _repositoryDeliverier.Create(deliverier);

        if (!resultCreate)
        {
            apiReponse.SetError(nameof(resultCreate).AppendError(AdditionalMessageEnum.Unavailable));
        }

        apiReponse.SetData(deliverier);

        return apiReponse;
    }
}
