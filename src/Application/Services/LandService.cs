using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class LandService(ApplicationDbContext db)
{
    public async Task<List<LandParcelListDto>> GetByFarmAsync(int farmId, CancellationToken ct = default)
    {
        return await db.LandParcels
            .Where(l => l.FarmId == farmId)
            .Include(l => l.CropSeasons)
            .Include(l => l.Farm)
            .AsNoTracking()
            .Select(l => new LandParcelListDto
            {
                Id = l.Id,
                FarmId = l.FarmId,
                FarmName = l.Farm.FarmName,
                LandName = l.LandName,
                Area_Decimal = l.Area_Decimal,
                OwnershipType = l.OwnershipType,
                SoilType = l.SoilType,
                ActiveCropSeasons = l.CropSeasons.Count(c => !c.IsDeleted && c.Status != Domain.Enums.CropSeasonStatus.Closed)
            }).ToListAsync(ct);
    }

    public async Task<int> CreateLandAsync(LandParcelCreateDto dto, string userId, CancellationToken ct = default)
    {
        var land = new LandParcel
        {
            FarmId = dto.FarmId,
            LandName = dto.LandName,
            Area_Decimal = dto.Area_Decimal,
            OwnershipType = dto.OwnershipType,
            LeaseCostPerSeason = dto.LeaseCostPerSeason,
            SoilType = dto.SoilType,
            LastTestedDate = dto.LastTestedDate,
            CreatedBy = userId
        };
        db.LandParcels.Add(land);
        await db.SaveChangesAsync(ct);
        return land.Id;
    }

    public async Task<bool> UpdateLandAsync(LandParcelEditDto dto, string userId, CancellationToken ct = default)
    {
        var land = await db.LandParcels.FindAsync([dto.Id], ct);
        if (land == null) return false;
        land.LandName = dto.LandName;
        land.Area_Decimal = dto.Area_Decimal;
        land.OwnershipType = dto.OwnershipType;
        land.LeaseCostPerSeason = dto.LeaseCostPerSeason;
        land.SoilType = dto.SoilType;
        land.LastTestedDate = dto.LastTestedDate;
        land.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteLandAsync(int id, string userId, CancellationToken ct = default)
    {
        var land = await db.LandParcels.FindAsync([id], ct);
        if (land == null) return false;
        land.IsDeleted = true;
        land.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<LandParcelEditDto?> GetLandByIdAsync(int id, CancellationToken ct = default)
    {
        var l = await db.LandParcels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (l == null) return null;
        return new LandParcelEditDto
        {
            Id = l.Id,
            FarmId = l.FarmId,
            LandName = l.LandName,
            Area_Decimal = l.Area_Decimal,
            OwnershipType = l.OwnershipType,
            LeaseCostPerSeason = l.LeaseCostPerSeason,
            SoilType = l.SoilType,
            LastTestedDate = l.LastTestedDate
        };
    }

    // Crop Seasons
    public async Task<List<CropSeasonListDto>> GetCropSeasonsByLandAsync(int landId, CancellationToken ct = default)
    {
        return await db.CropSeasons
            .Where(c => c.LandId == landId)
            .Include(c => c.LandParcel)
            .AsNoTracking()
            .Select(c => new CropSeasonListDto
            {
                Id = c.Id,
                LandId = c.LandId,
                LandName = c.LandParcel.LandName,
                CropName = c.CropName,
                Variety = c.Variety,
                SowDate = c.SowDate,
                ExpectedHarvestDate = c.ExpectedHarvestDate,
                Status = c.Status,
                ExpectedYield_kg = c.ExpectedYield_kg,
                ActualYield_kg = c.ActualYield_kg
            }).ToListAsync(ct);
    }

    public async Task<int> CreateCropSeasonAsync(CropSeasonCreateDto dto, string userId, CancellationToken ct = default)
    {
        var season = new CropSeason
        {
            LandId = dto.LandId,
            CropName = dto.CropName,
            Variety = dto.Variety,
            SowDate = dto.SowDate,
            ExpectedHarvestDate = dto.ExpectedHarvestDate,
            ExpectedYield_kg = dto.ExpectedYield_kg,
            SeedCost = dto.SeedCost,
            CreatedBy = userId
        };
        db.CropSeasons.Add(season);
        await db.SaveChangesAsync(ct);
        return season.Id;
    }

    public async Task<bool> UpdateCropSeasonAsync(CropSeasonEditDto dto, string userId, CancellationToken ct = default)
    {
        var season = await db.CropSeasons.FindAsync([dto.Id], ct);
        if (season == null) return false;
        season.CropName = dto.CropName;
        season.Variety = dto.Variety;
        season.SowDate = dto.SowDate;
        season.ExpectedHarvestDate = dto.ExpectedHarvestDate;
        season.ActualHarvestDate = dto.ActualHarvestDate;
        season.ExpectedYield_kg = dto.ExpectedYield_kg;
        season.ActualYield_kg = dto.ActualYield_kg;
        season.SeedCost = dto.SeedCost;
        season.Status = dto.Status;
        season.ModifiedBy = userId;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
