using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.GetOne;

public class MotorcycleGetOneQueryHandler : IRequestHandler<MotorcycleGetOneQueryCommand, ActionResult>
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;

    public MotorcycleGetOneQueryHandler(IMotorcycleRepository repositoryMotorcycle)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
    }

    public async ValueTask<ActionResult> Handle(MotorcycleGetOneQueryCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();

        var motorcycle = await _repositoryMotorcycle.GetOneByIdAsync(request.Id);

        if (motorcycle == null)
        {
            apiReponse.SetError(nameof(motorcycle).AppendError(AdditionalMessageEnum.NotFound));

            return apiReponse;
        }

        var motorcycleDtos = MotorcycleServiceMappers.MapEntityToDto(motorcycle);

        apiReponse.SetData(motorcycleDtos);

        return apiReponse;
    }
}
