using CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;

namespace CoreGoDelivery.Domain.Repositories.GoDelivery;

public interface IMotorcycleRepository
{
    Task<(List<MotorcycleEntity> Items, int TotalCount)> List(
        string? plate,
        CoreGoDelivery.Domain.Enums.Pagination.MotorcycleListFindBy? findBy,
        string? valueToFind,
        CoreGoDelivery.Domain.Enums.Pagination.OrderByEnum orderBy,
        int page,
        int limit);
    Task<MotorcycleEntity?> GetOneByIdAsync(string id);
    Task<bool> CheckIsUnicById(string id);
    Task<bool> CheckIsUnicByPlateAsync(string plate);
    Task<bool> Create(MotorcycleEntity data);
    Task<bool> DeleteById(string id);
    Task<bool> ChangePlateByIdAsync(string id, string plate);
}
