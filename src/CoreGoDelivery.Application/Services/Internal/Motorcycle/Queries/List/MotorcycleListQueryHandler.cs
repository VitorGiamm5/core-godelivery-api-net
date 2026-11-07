using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;
using CoreGoDelivery.Domain.Enums.Pagination;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Response.BaseResponse;
using Mediator;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Queries.List;

public class MotorcycleListQueryHandler : IRequestHandler<MotorcycleListQueryCommand, ActionResult>
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;

    public MotorcycleListQueryHandler(
        IMotorcycleRepository repositoryMotorcycle)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
    }

    public async ValueTask<ActionResult> Handle(MotorcycleListQueryCommand request, CancellationToken cancellationToken)
    {
        var apiReponse = new ActionResult();

        var validationError = Validate(request);
        if (validationError is not null)
        {
            apiReponse.SetError(validationError);
            return apiReponse;
        }

        request.Plate = request.Plate.RemoveCharactersToUpper();
        if (request.FindyBy == MotorcycleListFindBy.model_name)
        {
            request.ValueToFind = request.ValueToFind!.RemoveCharactersToUpper();
        }

        var result = await _repositoryMotorcycle.List(
            request.Plate,
            request.FindyBy,
            request.ValueToFind,
            request.OrderBy,
            request.Page,
            request.Limit);

        var motorcycleDtos = MotorcycleServiceMappers.MapEntityListToDto(result.Items);

        apiReponse.SetPaginedData(motorcycleDtos, result.TotalCount, request.Page, request.Limit);

        return apiReponse;
    }

    private static string? Validate(MotorcycleListQueryCommand request)
    {
        if (request.Page < 1 || request.Limit is < 1 or > 100 || request.Page > int.MaxValue / request.Limit)
        {
            return "Invalid page or limit.";
        }

        if (!Enum.IsDefined(typeof(OrderByEnum), request.OrderBy))
        {
            return "Invalid orderBy.";
        }

        if (request.FindyBy is null)
        {
            return string.IsNullOrWhiteSpace(request.ValueToFind) ? null : "findyBy is required when valueToFind is supplied.";
        }

        if (!Enum.IsDefined(typeof(MotorcycleListFindBy), request.FindyBy.Value) || string.IsNullOrWhiteSpace(request.ValueToFind))
        {
            return "A supported findyBy and valueToFind are required together.";
        }

        var requiresYear = request.FindyBy is MotorcycleListFindBy.manufacture_year
            or MotorcycleListFindBy.manufacture_year_start
            or MotorcycleListFindBy.manufacture_year_end;
        if (requiresYear && !int.TryParse(request.ValueToFind, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            return "valueToFind must be a valid manufacture year.";
        }

        if (request.FindyBy == MotorcycleListFindBy.create_date &&
            !DateTimeOffset.TryParse(
                request.ValueToFind,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                out _))
        {
            return "valueToFind must be a valid create date.";
        }

        return null;
    }
}
