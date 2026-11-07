using CoreGoDelivery.Application.Extensions;
using CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Create;
using CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;
using CoreGoDelivery.Domain.NotificationMotorcycle;

namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;

public static class MotorcycleServiceMappers
{
    public static List<MotorcycleDto> MapEntityListToDto(List<MotorcycleEntity>? entity)
    {
        List<MotorcycleDto> motorcycleDtos = [];

        if (entity != null)
        {
            motorcycleDtos = entity
                 .Select(motorcycle => MapEntityToDto(motorcycle))
                 .ToList();
        }

        return motorcycleDtos;
    }

    public static MotorcycleDto MapEntityToDto(MotorcycleEntity motorcycle)
    {
        var restult = new MotorcycleDto(
            motorcycle.Id,
            motorcycle.YearManufacture,
            motorcycle.MotorcycleModel!.Name,
            motorcycle.PlateNormalized);

        return restult;
    }

    public static MotorcycleEntity MapCreateToEntity(MotorcycleCreateCommand data)
    {
        var result = new MotorcycleEntity()
        {
            Id = data.Id,
            YearManufacture = data.YearManufacture,
            MotorcycleModelId = data.ModelName,
            PlateNormalized = data.Plate.RemoveCharactersToUpper()
        };

        return result;
    }

    public static NotificationMotorcycleDto MapNotificationDto(MotorcycleEntity data)
    {
        var result = new NotificationMotorcycleDto()
        {
            Id = data.Id,
            YearManufacture = data.YearManufacture,
            ModelMotorcycleId = data.MotorcycleModelId,
            MotorcycleId = data.Id,
            PlateNormalized = data.PlateNormalized,
            DateCreated = data.DateCreated
        };

        return result;
    }
}
