using CoreGoDelivery.Domain.Entities.GoDelivery.MotorcycleModel;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.MotorcycleModel.Queries.List;

public class MotorcycleModelQueryHandler : IRequestHandler<MotorcycleModelQueryCommand, ActionResult>
{
    public readonly IMotorcycleModelRepository _repositoryMotorcycleModel;

    public MotorcycleModelQueryHandler(IMotorcycleModelRepository repositoryMotorcycleModel)
    {
        _repositoryMotorcycleModel = repositoryMotorcycleModel;
    }

    public async ValueTask<ActionResult> Handle(MotorcycleModelQueryCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();

        List<MotorcycleModelEntity> result = await _repositoryMotorcycleModel.GetAll(request.Name, request.Id);

        apiReponse.SetPaginedData(result, result.Count, 1, Math.Max(result.Count, 1));

        return apiReponse;
    }
}
