using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public record DashboardStats(
    int TotalFarms,
    int ActiveBatches,
    int TotalAnimals,
    decimal TotalInvestment,
    decimal TotalRevenue,
    decimal GrossProfit,
    int PendingTasks,
    int EidTargetBatches
);

public class DashboardService(ApplicationDbContext db)
{
    public async Task<DashboardStats> GetStatsAsync(string userId, bool isSuperAdmin, CancellationToken ct = default)
    {
        IQueryable<int> farmIds;
        if (isSuperAdmin)
            farmIds = db.Farms.Select(f => f.Id);
        else
            farmIds = db.UserFarms.Where(uf => uf.UserId == userId).Select(uf => uf.FarmId);

        var ids = await farmIds.ToListAsync(ct);

        var totalFarms = ids.Count;
        var activeBatches = await db.Batches.CountAsync(b => ids.Contains(b.FarmId) && b.Status == BatchStatus.Active, ct);
        var totalAnimals = await db.Batches.Where(b => ids.Contains(b.FarmId) && b.Status == BatchStatus.Active).SumAsync(b => (int?)b.InitialCount, ct) ?? 0;
        var totalInvestment = await db.Investments.Where(i => ids.Contains(i.FarmId)).SumAsync(i => (decimal?)i.Amount, ct) ?? 0;
        var totalRevenue = await db.Sales.Where(s => ids.Contains(s.Batch!.FarmId)).SumAsync(s => (decimal?)s.TotalRevenue, ct) ?? 0;
        var totalCosts = await db.Costs.Where(c => ids.Contains(c.FarmId)).SumAsync(c => (decimal?)c.Amount, ct) ?? 0;
        var purchaseCosts = await db.Batches.Where(b => ids.Contains(b.FarmId)).SumAsync(b => (decimal?)b.PurchaseCost, ct) ?? 0;
        var grossProfit = totalRevenue - totalCosts - purchaseCosts;
        var pendingTasks = await db.DailyTasks.CountAsync(t => ids.Contains(t.FarmId) && t.Status == Domain.Enums.TaskStatus.Pending && t.TaskDate == DateTime.Today, ct);
        var eidTargets = await db.Batches.CountAsync(b => ids.Contains(b.FarmId) && b.IsEidTarget && b.Status == BatchStatus.Active, ct);

        return new DashboardStats(totalFarms, activeBatches, totalAnimals, totalInvestment, totalRevenue, grossProfit, pendingTasks, eidTargets);
    }
}
