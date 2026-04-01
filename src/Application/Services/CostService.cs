using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class CostService(ApplicationDbContext db)
{
    public async Task<List<CostListDto>> GetByFarmAsync(int farmId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var query = db.Costs
            .Where(c => c.FarmId == farmId)
            .Include(c => c.Batch)
            .Include(c => c.CropSeason)
            .AsNoTracking();

        if (from.HasValue) query = query.Where(c => c.CostDate >= from.Value);
        if (to.HasValue) query = query.Where(c => c.CostDate <= to.Value);

        return await query.OrderByDescending(c => c.CostDate).Select(c => new CostListDto
        {
            Id = c.Id,
            CostCategory = c.CostCategory,
            Description = c.Description,
            CostDate = c.CostDate,
            Amount = c.Amount,
            IsActual = c.IsActual,
            BatchName = c.Batch != null ? c.Batch.BatchName : null,
            CropName = c.CropSeason != null ? c.CropSeason.CropName : null
        }).ToListAsync(ct);
    }

    public async Task<int> CreateAsync(CostCreateDto dto, string userId, CancellationToken ct = default)
    {
        var cost = new Cost
        {
            FarmId = dto.FarmId,
            BatchId = dto.BatchId,
            CropSeasonId = dto.CropSeasonId,
            CostCategory = dto.CostCategory,
            Description = dto.Description,
            CostDate = dto.CostDate,
            Amount = dto.Amount,
            IsActual = dto.IsActual,
            EnteredById = userId,
            CreatedBy = userId
        };
        db.Costs.Add(cost);
        await db.SaveChangesAsync(ct);
        return cost.Id;
    }

    public async Task<bool> UpdateAsync(CostEditDto dto, string userId, CancellationToken ct = default)
    {
        var cost = await db.Costs.FindAsync([dto.Id], ct);
        if (cost == null) return false;

        cost.CostCategory = dto.CostCategory;
        cost.Description = dto.Description;
        cost.CostDate = dto.CostDate;
        cost.Amount = dto.Amount;
        cost.IsActual = dto.IsActual;
        cost.ModifiedBy = userId;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var cost = await db.Costs.FindAsync([id], ct);
        if (cost == null) return false;
        cost.IsDeleted = true;
        cost.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<CostEditDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await db.Costs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (c == null) return null;
        return new CostEditDto
        {
            Id = c.Id,
            FarmId = c.FarmId,
            BatchId = c.BatchId,
            CropSeasonId = c.CropSeasonId,
            CostCategory = c.CostCategory,
            Description = c.Description,
            CostDate = c.CostDate,
            Amount = c.Amount,
            IsActual = c.IsActual
        };
    }
}
