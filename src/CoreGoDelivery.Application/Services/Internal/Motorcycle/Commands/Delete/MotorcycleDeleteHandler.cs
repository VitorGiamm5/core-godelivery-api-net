using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Delete;

public class MotorcycleDeleteHandler : IRequestHandler<MotorcycleDeleteCommand, ActionResult>
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;

    private readonly MotorcycleDeleteValidator _validator;

    public MotorcycleDeleteHandler(
        IMotorcycleRepository repositoryMotorcycle,
        MotorcycleDeleteValidator validator)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
        _validator = validator;
    }

    public async ValueTask<ActionResult> Handle(MotorcycleDeleteCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();
        apiReponse.SetError(await _validator.BuilderDeleteValidator(request.Id));

        if (apiReponse.HasError())
        {
            return apiReponse;
        }

        var deleted = await _repositoryMotorcycle.DeleteById(request.Id);
        if (!deleted)
        {
            apiReponse.SetError("motorcycleId is invalid");
            return apiReponse;
        }

        apiReponse.SetData(new { id = request.Id, deleted = true });

        return apiReponse!;
    }
}
