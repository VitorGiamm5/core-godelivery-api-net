using CoreGoDelivery.Domain.Entities.GoDelivery.Deliverier;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CoreGoDelivery.Infrastructure.Repositories.GoDelivery;

public class DeliverierRepository : BaseRepository<DeliverierEntity>, IDeliverierRepository
{
    public DeliverierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> CheckIsUnicById(string id)
    {
        var result = await _context.Set<DeliverierEntity>()
            .FirstOrDefaultAsync(x => x.Id == id);

        return !HasValue(result);
    }

    public async Task<DeliverierEntity?> GetOneById(string id)
    {
        var result = await _context.Set<DeliverierEntity>()
            .FirstOrDefaultAsync(x => x.Id == id);

        return result;
    }

    public async Task<bool> CheckIsUnicByCnpj(string id)
    {
        var result = await _context.Set<DeliverierEntity>()
            .FirstOrDefaultAsync(x => x.Cnpj == id);

        return !HasValue(result);
    }

    public async Task<bool> Create(DeliverierEntity data)
    {
        var result = await _context
            .Set<DeliverierEntity>()
            .AddAsync(data);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (PostgreSqlConstraintErrors.IsUniqueViolation(exception))
        {
            return false;
        }

        return IsSuccessCreate(result);
    }

    public async Task<DeliverierEntity?> GetOneByIdLicense(string id)
    {
        var result = await _context.Set<DeliverierEntity>()
            .FirstOrDefaultAsync(x => x.LicenceDriverId == id);

        return result;
    }
}
