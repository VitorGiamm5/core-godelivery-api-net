using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Delete;

public class MotorcycleDeleteValidator
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;
    public readonly IRentalRepository _rentalRepository;

    public MotorcycleDeleteValidator(
        IMotorcycleRepository repositoryMotorcycle,
        IRentalRepository rentalRepository)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
        _rentalRepository = rentalRepository;
    }

    public async Task<StringBuilder> BuilderDeleteValidator(string? idMotorcycle)
    {
        var message = new StringBuilder();

        if (string.IsNullOrEmpty(idMotorcycle))
        {
            message.Append(nameof(idMotorcycle));
            return message;
        }

        var motorcycle = await _repositoryMotorcycle.GetOneByIdAsync(idMotorcycle);
        if (motorcycle == null)
        {
            message.Append(nameof(idMotorcycle).AppendError(AdditionalMessageEnum.NotFound));
            return message;
        }

        var rental = await _rentalRepository.FindByMotorcycleId(idMotorcycle);
        if (rental != null)
        {
            message.Append(nameof(rental).AppendError(AdditionalMessageEnum.Unavailable));
        }

        return message;
    }
}
