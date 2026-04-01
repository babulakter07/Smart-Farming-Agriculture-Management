using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class FarmService(ApplicationDbContext db)
{
    public async Task<List<FarmListDto>> GetAllAsync(string userId, bool isSuperAdmin, CancellationToken ct = default)
    {
        var query = db.Farms
            .Include(f => f.Batches)
            .AsNoTracking();

        if (!isSuperAdmin)
            query = query.Where(f => f.OwnerId == userId || f.UserFarms.Any(uf => uf.UserId == userId));

        return await query.Select(f => new FarmListDto
        {
            Id = f.Id,
            FarmName = f.FarmName,
            FarmType = f.FarmType,
            Location = f.Location,
            IsActive = f.IsActive,
            TotalArea = f.TotalArea,
            BatchCount = f.Batches.Count(b => !b.IsDeleted)
        }).ToListAsync(ct);
    }

    public async Task<FarmDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var farm = await db.Farms
            .Include(f => f.Owner)
            .Include(f => f.Batches)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (farm == null) return null;

        return new FarmDetailDto
        {
            Id = farm.Id,
            FarmName = farm.FarmName,
            FarmType = farm.FarmType,
            Location = farm.Location,
            Latitude = farm.Latitude,
            Longitude = farm.Longitude,
            TotalArea = farm.TotalArea,
            IsActive = farm.IsActive,
            OwnerName = farm.Owner?.FullName ?? farm.Owner?.Email ?? "Unknown",
            CreatedDate = farm.CreatedDate,
            Batches = farm.Batches.Where(b => !b.IsDeleted).Select(b => new BatchListDto
            {
                Id = b.Id,
                BatchName = b.BatchName,
                Species = b.Species,
                Breed = b.Breed,
                StartDate = b.StartDate,
                InitialCount = b.InitialCount,
                Status = b.Status,
                IsEidTarget = b.IsEidTarget,
                PurchaseCost = b.PurchaseCost
            }).ToList()
        };
    }

    public async Task<int> CreateAsync(FarmCreateDto dto, string ownerId, CancellationToken ct = default)
    {
        var farm = new Farm
        {
            OwnerId = ownerId,
            FarmName = dto.FarmName,
            FarmType = dto.FarmType,
            Location = dto.Location,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            TotalArea = dto.TotalArea,
            IsActive = true,
            CreatedBy = ownerId
        };
        db.Farms.Add(farm);
        await db.SaveChangesAsync(ct);

        // Add owner to UserFarm as well
        db.UserFarms.Add(new UserFarm { UserId = ownerId, FarmId = farm.Id, CreatedBy = ownerId });
        await db.SaveChangesAsync(ct);

        return farm.Id;
    }

    public async Task<bool> UpdateAsync(FarmEditDto dto, string userId, CancellationToken ct = default)
    {
        var farm = await db.Farms.FindAsync([dto.Id], ct);
        if (farm == null) return false;

        farm.FarmName = dto.FarmName;
        farm.FarmType = dto.FarmType;
        farm.Location = dto.Location;
        farm.Latitude = dto.Latitude;
        farm.Longitude = dto.Longitude;
        farm.TotalArea = dto.TotalArea;
        farm.IsActive = dto.IsActive;
        farm.ModifiedBy = userId;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var farm = await db.Farms.FindAsync([id], ct);
        if (farm == null) return false;

        farm.IsDeleted = true;
        farm.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
