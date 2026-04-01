using Firming_Solution.Application.DTOs;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class DashboardService(ApplicationDbContext db)
{
    public async Task<DashboardDto> GetDashboardAsync(string userId, bool isSuperAdmin, CancellationToken ct = default)
    {
        IQueryable<Domain.Entities.Farm> farmQuery = db.Farms;
        if (!isSuperAdmin)
            farmQuery = farmQuery.Where(f => f.OwnerId == userId || f.UserFarms.Any(uf => uf.UserId == userId));

        var farmIds = await farmQuery.Select(f => f.Id).ToListAsync(ct);

        var totalInvestment = await db.Investments
            .Where(i => farmIds.Contains(i.FarmId))
            .SumAsync(i => (decimal?)i.Amount, ct) ?? 0;

        var totalRevenue = await db.Sales
            .Where(s => farmIds.Contains(s.Batch.FarmId))
            .SumAsync(s => (decimal?)s.TotalRevenue, ct) ?? 0;

        var totalCosts = await db.Costs
            .Where(c => farmIds.Contains(c.FarmId) && c.IsActual)
            .SumAsync(c => (decimal?)c.Amount, ct) ?? 0;

        var feedCosts = await db.DailyFeedLogs
            .Where(f => farmIds.Contains(f.Batch.FarmId))
            .SumAsync(f => (decimal?)(f.Quantity_kg * f.PricePerKg), ct) ?? 0;

        var activeBatches = await db.Batches
            .Where(b => farmIds.Contains(b.FarmId) && b.Status == Domain.Enums.BatchStatus.Active)
            .CountAsync(ct);

        var eidTargets = await db.Batches
            .Where(b => farmIds.Contains(b.FarmId) && b.IsEidTarget && b.Status != Domain.Enums.BatchStatus.Closed)
            .CountAsync(ct);

        var pendingTasks = await db.DailyTasks
            .Where(t => farmIds.Contains(t.FarmId) && t.Status == Domain.Enums.TaskStatus.Pending)
            .CountAsync(ct);

        var recentBatches = await db.Batches
            .Where(b => farmIds.Contains(b.FarmId))
            .Include(b => b.MortalityLogs)
            .OrderByDescending(b => b.CreatedDate)
            .Take(5)
            .Select(b => new BatchListDto
            {
                Id = b.Id,
                BatchName = b.BatchName,
                Species = b.Species,
                Status = b.Status,
                StartDate = b.StartDate,
                InitialCount = b.InitialCount,
                PurchaseCost = b.PurchaseCost,
                IsEidTarget = b.IsEidTarget
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var recentSales = await db.Sales
            .Where(s => farmIds.Contains(s.Batch.FarmId))
            .Include(s => s.Batch)
            .Include(s => s.Buyer)
            .OrderByDescending(s => s.SaleDate)
            .Take(5)
            .Select(s => new SaleListDto
            {
                Id = s.Id,
                BatchId = s.BatchId,
                BatchName = s.Batch.BatchName,
                BuyerName = s.Buyer != null ? s.Buyer.Name : null,
                SaleDate = s.SaleDate,
                TotalRevenue = s.TotalRevenue,
                PricePerKg = s.PricePerKg,
                IsEidSale = s.IsEidSale
            })
            .AsNoTracking()
            .ToListAsync(ct);

        // Weather alerts: high temp or high rainfall
        var alerts = await db.WeatherLogs
            .Where(w => farmIds.Contains(w.FarmId) && w.LogDate >= DateTime.UtcNow.Date.AddDays(-3))
            .OrderByDescending(w => w.LogDate)
            .Take(3)
            .Select(w => $"Weather alert on {w.LogDate:dd MMM}: {w.WeatherCondition}, Temp max {w.TempMax_C}°C")
            .ToListAsync(ct);

        return new DashboardDto
        {
            TotalFarms = farmIds.Count,
            ActiveBatches = activeBatches,
            TotalInvestment = totalInvestment,
            TotalRevenue = totalRevenue,
            TotalCosts = totalCosts + feedCosts,
            NetProfit = totalRevenue - (totalCosts + feedCosts),
            PendingTasks = pendingTasks,
            EidTargetBatches = eidTargets,
            RecentBatches = recentBatches,
            RecentSales = recentSales,
            WeatherAlerts = alerts
        };
    }
}
