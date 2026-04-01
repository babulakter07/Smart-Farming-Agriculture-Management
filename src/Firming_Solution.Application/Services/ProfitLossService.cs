using Firming_Solution.Application.Interfaces;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class ProfitLossService(ApplicationDbContext db) : IProfitLossService
{
    public async Task<BatchPLSummary?> GetBatchPLAsync(int batchId, CancellationToken ct = default)
    {
        var batch = await db.Batches
            .Include(b => b.FeedLogs)
            .Include(b => b.Costs)
            .Include(b => b.Sales)
            .Include(b => b.MortalityLogs)
            .FirstOrDefaultAsync(b => b.Id == batchId, ct);

        if (batch is null) return null;

        var totalFeedCost = batch.FeedLogs.Sum(f => f.Quantity_kg * f.PricePerKg);
        var totalMedCost = batch.Costs.Where(c => c.CostCategory == CostCategory.Medicine).Sum(c => c.Amount);
        var totalLabour = batch.Costs.Where(c => c.CostCategory == CostCategory.Labour).Sum(c => c.Amount);
        var totalOther = batch.Costs.Where(c => c.CostCategory != CostCategory.Medicine && c.CostCategory != CostCategory.Labour && c.CostCategory != CostCategory.Feed).Sum(c => c.Amount);
        var totalRevenue = batch.Sales.Sum(s => s.TotalRevenue);
        var totalDeaths = batch.MortalityLogs.Sum(m => m.Count);
        var liveCount = batch.InitialCount - totalDeaths;

        var totalCost = batch.PurchaseCost + totalFeedCost + totalMedCost + totalLabour + totalOther;
        var grossProfit = totalRevenue - totalCost;
        var roi = totalCost > 0 ? Math.Round((grossProfit / totalCost) * 100, 2) : 0;
        var costPerHead = liveCount > 0 ? Math.Round(totalCost / liveCount, 2) : 0;
        var latestWeight = batch.Id > 0
            ? await db.WeightLogs
                .Where(w => w.BatchId == batchId)
                .OrderByDescending(w => w.LogDate)
                .Select(w => (decimal?)w.AvgWeight_kg)
                .FirstOrDefaultAsync(ct)
            : null;
        var breakeven = latestWeight.HasValue && latestWeight > 0 ? Math.Round(costPerHead / latestWeight.Value, 2) : (decimal?)null;

        return new BatchPLSummary(
            batch.Id, batch.BatchName, batch.Species.ToString(),
            batch.PurchaseCost, totalFeedCost, totalMedCost, totalLabour, totalOther,
            totalRevenue, grossProfit, roi, batch.InitialCount, liveCount, costPerHead, breakeven
        );
    }

    public async Task<IList<BatchPLSummary>> GetFarmPLAsync(int farmId, CancellationToken ct = default)
    {
        var batchIds = await db.Batches
            .Where(b => b.FarmId == farmId)
            .Select(b => b.Id)
            .ToListAsync(ct);

        var results = new List<BatchPLSummary>();
        foreach (var id in batchIds)
        {
            var summary = await GetBatchPLAsync(id, ct);
            if (summary is not null) results.Add(summary);
        }
        return results;
    }
}
