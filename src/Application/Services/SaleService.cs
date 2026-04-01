using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class SaleService(ApplicationDbContext db)
{
    public async Task<List<SaleListDto>> GetByBatchAsync(int batchId, CancellationToken ct = default)
    {
        return await db.Sales
            .Where(s => s.BatchId == batchId)
            .Include(s => s.Buyer)
            .AsNoTracking()
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleListDto
            {
                Id = s.Id,
                BatchId = s.BatchId,
                BatchName = s.Batch.BatchName,
                BuyerName = s.Buyer != null ? s.Buyer.Name : null,
                SaleDate = s.SaleDate,
                Quantity = s.Quantity,
                TotalWeight_kg = s.TotalWeight_kg,
                PricePerKg = s.PricePerKg,
                TotalRevenue = s.TotalRevenue,
                IsEidSale = s.IsEidSale
            }).ToListAsync(ct);
    }

    public async Task<List<SaleListDto>> GetByFarmAsync(int farmId, CancellationToken ct = default)
    {
        return await db.Sales
            .Where(s => s.Batch.FarmId == farmId)
            .Include(s => s.Batch)
            .Include(s => s.Buyer)
            .AsNoTracking()
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleListDto
            {
                Id = s.Id,
                BatchId = s.BatchId,
                BatchName = s.Batch.BatchName,
                BuyerName = s.Buyer != null ? s.Buyer.Name : null,
                SaleDate = s.SaleDate,
                Quantity = s.Quantity,
                TotalWeight_kg = s.TotalWeight_kg,
                PricePerKg = s.PricePerKg,
                TotalRevenue = s.TotalRevenue,
                IsEidSale = s.IsEidSale
            }).ToListAsync(ct);
    }

    public async Task<int> CreateAsync(SaleCreateDto dto, string userId, CancellationToken ct = default)
    {
        var sale = new Sale
        {
            BatchId = dto.BatchId,
            BuyerId = dto.BuyerId,
            SaleDate = dto.SaleDate,
            Quantity = dto.Quantity,
            TotalWeight_kg = dto.TotalWeight_kg,
            PricePerKg = dto.PricePerKg,
            TotalRevenue = dto.TotalRevenue,
            IsEidSale = dto.IsEidSale,
            Notes = dto.Notes,
            CreatedBy = userId
        };
        db.Sales.Add(sale);
        await db.SaveChangesAsync(ct);
        return sale.Id;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var sale = await db.Sales.FindAsync([id], ct);
        if (sale == null) return false;
        sale.IsDeleted = true;
        sale.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<SaleEditDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var s = await db.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s == null) return null;
        return new SaleEditDto
        {
            Id = s.Id,
            BatchId = s.BatchId,
            BuyerId = s.BuyerId,
            SaleDate = s.SaleDate,
            Quantity = s.Quantity,
            TotalWeight_kg = s.TotalWeight_kg,
            PricePerKg = s.PricePerKg,
            TotalRevenue = s.TotalRevenue,
            IsEidSale = s.IsEidSale,
            Notes = s.Notes
        };
    }
}
