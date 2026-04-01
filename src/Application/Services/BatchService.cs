using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class BatchService(ApplicationDbContext db)
{
    public async Task<List<BatchListDto>> GetByFarmAsync(int farmId, CancellationToken ct = default)
    {
        var batches = await db.Batches
            .Where(b => b.FarmId == farmId)
            .Include(b => b.MortalityLogs)
            .AsNoTracking()
            .ToListAsync(ct);

        return batches.Select(b => new BatchListDto
        {
            Id = b.Id,
            BatchName = b.BatchName,
            Species = b.Species,
            Breed = b.Breed,
            StartDate = b.StartDate,
            InitialCount = b.InitialCount,
            Status = b.Status,
            IsEidTarget = b.IsEidTarget,
            PurchaseCost = b.PurchaseCost,
            LiveCount = b.InitialCount - b.MortalityLogs.Where(m => !m.IsDeleted).Sum(m => m.Count)
        }).ToList();
    }

    public async Task<BatchDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var b = await db.Batches
            .Include(x => x.Farm)
            .Include(x => x.DailyFeedLogs)
            .Include(x => x.MortalityLogs)
            .Include(x => x.Sales)
            .Include(x => x.Costs)
            .Include(x => x.WeightLogs.OrderByDescending(w => w.LogDate))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (b == null) return null;

        var feedCost = b.DailyFeedLogs.Where(f => !f.IsDeleted).Sum(f => f.Quantity_kg * f.PricePerKg);
        var otherCost = b.Costs.Where(c => !c.IsDeleted && c.IsActual).Sum(c => c.Amount);
        var revenue = b.Sales.Where(s => !s.IsDeleted).Sum(s => s.TotalRevenue);
        var totalCost = feedCost + otherCost + b.PurchaseCost;
        var mortalities = b.MortalityLogs.Where(m => !m.IsDeleted).Sum(m => m.Count);
        var latestWeight = b.WeightLogs.FirstOrDefault();

        return new BatchDetailDto
        {
            Id = b.Id,
            FarmId = b.FarmId,
            BatchName = b.BatchName,
            Species = b.Species,
            Breed = b.Breed,
            StartDate = b.StartDate,
            PlannedEndDate = b.PlannedEndDate,
            InitialCount = b.InitialCount,
            InitialWeight_kg = b.InitialWeight_kg,
            PurchaseCost = b.PurchaseCost,
            Status = b.Status,
            IsEidTarget = b.IsEidTarget,
            Notes = b.Notes,
            FarmName = b.Farm?.FarmName ?? "",
            TotalFeedCost = feedCost,
            TotalOtherCost = otherCost,
            TotalRevenue = revenue,
            GrossProfit = revenue - totalCost,
            CurrentFCR = latestWeight?.FCR_Cumulative,
            CurrentAvgWeight = latestWeight?.AvgWeight_kg,
            LiveCount = b.InitialCount - mortalities,
            TotalMortalities = mortalities
        };
    }

    public async Task<int> CreateAsync(BatchCreateDto dto, string userId, CancellationToken ct = default)
    {
        var batch = new Batch
        {
            FarmId = dto.FarmId,
            BatchName = dto.BatchName,
            Species = dto.Species,
            Breed = dto.Breed,
            StartDate = dto.StartDate,
            PlannedEndDate = dto.PlannedEndDate,
            InitialCount = dto.InitialCount,
            InitialWeight_kg = dto.InitialWeight_kg,
            PurchaseCost = dto.PurchaseCost,
            IsEidTarget = dto.IsEidTarget,
            Notes = dto.Notes,
            CreatedBy = userId
        };
        db.Batches.Add(batch);
        await db.SaveChangesAsync(ct);

        // Auto-create purchase cost entry
        db.Costs.Add(new Cost
        {
            FarmId = dto.FarmId,
            BatchId = batch.Id,
            CostCategory = Domain.Enums.CostCategory.Other,
            Description = $"Initial purchase cost for batch: {dto.BatchName}",
            CostDate = dto.StartDate,
            Amount = dto.PurchaseCost,
            IsActual = true,
            EnteredById = userId,
            CreatedBy = userId
        });
        await db.SaveChangesAsync(ct);

        return batch.Id;
    }

    public async Task<bool> UpdateAsync(BatchEditDto dto, string userId, CancellationToken ct = default)
    {
        var batch = await db.Batches.FindAsync([dto.Id], ct);
        if (batch == null) return false;

        batch.BatchName = dto.BatchName;
        batch.Species = dto.Species;
        batch.Breed = dto.Breed;
        batch.StartDate = dto.StartDate;
        batch.PlannedEndDate = dto.PlannedEndDate;
        batch.InitialCount = dto.InitialCount;
        batch.InitialWeight_kg = dto.InitialWeight_kg;
        batch.PurchaseCost = dto.PurchaseCost;
        batch.Status = dto.Status;
        batch.IsEidTarget = dto.IsEidTarget;
        batch.Notes = dto.Notes;
        batch.ModifiedBy = userId;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var batch = await db.Batches.FindAsync([id], ct);
        if (batch == null) return false;
        batch.IsDeleted = true;
        batch.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
