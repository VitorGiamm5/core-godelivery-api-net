namespace CoreGoDelivery.Application.Services.Internal.Motorcycle.Commands.Commons;

public sealed record MotorcycleDto(
    string Id,
    int YearManufacture,
    string ModelName,
    string Plate);
