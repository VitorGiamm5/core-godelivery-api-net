using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;
using CoreGoDelivery.Domain.Enums.ServiceErrorMessage;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using System.Text;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.ChangePlateById;

public class MotorcycleChangePlateValidator
{
    public readonly IMotorcycleRepository _repositoryMotorcycle;

    public MotorcycleChangePlateValidator(IMotorcycleRepository repositoryMotorcycle)
    {
        _repositoryMotorcycle = repositoryMotorcycle;
    }

    public async Task<StringBuilder> ChangePlateValidator(MotorcycleChangePlateCommand command)
    {
        var message = new StringBuilder();

        var motorcycle = await _repositoryMotorcycle.GetOneByIdAsync(command.Id ?? string.Empty);
        if (motorcycle == null)
        {
            message.Append(nameof(command.Id).AppendError(AdditionalMessageEnum.NotFound));
            return message;
        }

        if (string.IsNullOrWhiteSpace(command.Plate))
        {
            message.Append(nameof(command.Plate).AppendError());
            return message;
        }

        var normalizedPlate = command.Plate.RemoveCharactersToUpper();
        if (!MotorcyclePlateValidator.Validator(normalizedPlate))
        {
            message.Append(nameof(command.Plate).AppendError(AdditionalMessageEnum.InvalidFormat));
            return message;
        }

        if (motorcycle.PlateNormalized == normalizedPlate)
        {
            return message;
        }

        if (!await _repositoryMotorcycle.CheckIsUnicByPlateAsync(normalizedPlate))
        {
            message.Append(nameof(command.Plate).AppendError(AdditionalMessageEnum.MustBeUnic));
        }

        return message;
    }
}
