using CoreGoDelivery.Domain.Entities.GoDelivery.Motorcycle;
using CoreGoDelivery.Domain.Repositories.GoDelivery;
using CoreGoDelivery.Domain.Entities.GoDelivery.NotificationMotorcycle;
using CoreGoDelivery.Domain.Enums.Pagination;
using CoreGoDelivery.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CoreGoDelivery.Infrastructure.Repositories.GoDelivery;

public class MotorcycleRepository : BaseRepository<MotorcycleEntity>, IMotorcycleRepository
{
    public MotorcycleRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<(List<MotorcycleEntity> Items, int TotalCount)> List(
        string? plate,
        MotorcycleListFindBy? findBy,
        string? valueToFind,
        OrderByEnum orderBy,
        int page,
        int limit)
    {
        IQueryable<MotorcycleEntity> query = _context.Set<MotorcycleEntity>()
            .Include(x => x.MotorcycleModel);

        if (!string.IsNullOrWhiteSpace(plate))
        {
            query = query.Where(x => x.PlateNormalized == plate);
        }

        if (findBy is not null && !string.IsNullOrWhiteSpace(valueToFind))
        {
            switch (findBy.Value)
            {
                case MotorcycleListFindBy.create_date:
                    var date = DateTimeOffset.Parse(valueToFind, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).UtcDateTime.Date;
                    query = query.Where(x => x.DateCreated >= date && x.DateCreated < date.AddDays(1));
                    break;
                case MotorcycleListFindBy.manufacture_year:
                    var year = int.Parse(valueToFind, CultureInfo.InvariantCulture);
                    query = query.Where(x => x.YearManufacture == year);
                    break;
                case MotorcycleListFindBy.manufacture_year_start:
                    var startYear = int.Parse(valueToFind, CultureInfo.InvariantCulture);
                    query = query.Where(x => x.YearManufacture >= startYear);
                    break;
                case MotorcycleListFindBy.manufacture_year_end:
                    var endYear = int.Parse(valueToFind, CultureInfo.InvariantCulture);
                    query = query.Where(x => x.YearManufacture <= endYear);
                    break;
                case MotorcycleListFindBy.model_name:
                    query = query.Where(x => x.MotorcycleModel != null && x.MotorcycleModel.NormalizedName == valueToFind);
                    break;
                case MotorcycleListFindBy.model_id:
                    query = query.Where(x => x.MotorcycleModelId == valueToFind);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(findBy), findBy, "Unsupported motorcycle filter.");
            }
        }

        var totalCount = await query.CountAsync();
        var orderedQuery = orderBy == OrderByEnum.asc
            ? query.OrderBy(x => x.DateCreated).ThenBy(x => x.Id)
            : query.OrderByDescending(x => x.DateCreated).ThenByDescending(x => x.Id);
        var items = await orderedQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<MotorcycleEntity?> GetOneByIdAsync(string id)
    {
        var entity = await _context.Set<MotorcycleEntity>()
            .Include(x => x.MotorcycleModel)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity;
    }

    public async Task<bool> CheckIsUnicById(string id)
    {
        var entity = await _context.Set<MotorcycleEntity>()
            .FirstOrDefaultAsync(x => x.Id == id);

        return IsUnic(entity);
    }

    public async Task<bool> CheckIsUnicByPlateAsync(string plate)
    {
        var entity = await _context.Set<MotorcycleEntity>()
            .FirstOrDefaultAsync(x => x.PlateNormalized == plate);

        return IsUnic(entity);
    }

    public async Task<bool> Create(MotorcycleEntity data)
    {
        var entity = await _context
            .Set<MotorcycleEntity>()
            .AddAsync(data);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (PostgreSqlConstraintErrors.IsUniqueViolation(exception))
        {
            return false;
        }

        return IsSuccessCreate(entity);
    }

    public async Task<bool> DeleteById(string id)
    {
        var motorcycle = await GetOneByIdAsync(id);

        if (motorcycle != null)
        {
            var notifications = await _context.Set<NotificationMotorcycleEntity>()
                .Where(notification => notification.IdMotorcycle == id)
                .ToListAsync();

            _context.Set<NotificationMotorcycleEntity>().RemoveRange(notifications);
            _context.Set<MotorcycleEntity>().Remove(motorcycle);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public async Task<bool> ChangePlateByIdAsync(string id, string plate)
    {
        var entity = await _context.Set<MotorcycleEntity>()
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return false;
        }
        else
        {
            entity.PlateNormalized = plate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (PostgreSqlConstraintErrors.IsUniqueViolation(exception))
            {
                return false;
            }

            return true;
        }
    }
}
