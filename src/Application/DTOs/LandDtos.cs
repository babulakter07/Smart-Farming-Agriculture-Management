using System.ComponentModel.DataAnnotations;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Application.DTOs;

public class LandParcelListDto
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public string FarmName { get; set; } = null!;
    public string LandName { get; set; } = null!;
    public decimal Area_Decimal { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public string? SoilType { get; set; }
    public int ActiveCropSeasons { get; set; }
}

public class LandParcelCreateDto
{
    public int FarmId { get; set; }
    [Required, MaxLength(150)]
    public string LandName { get; set; } = null!;
    [Range(0.001, double.MaxValue)]
    public decimal Area_Decimal { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public decimal? LeaseCostPerSeason { get; set; }
    public string? SoilType { get; set; }
    public DateTime? LastTestedDate { get; set; }
}

public class LandParcelEditDto : LandParcelCreateDto
{
    public int Id { get; set; }
}

public class CropSeasonListDto
{
    public int Id { get; set; }
    public int LandId { get; set; }
    public string LandName { get; set; } = null!;
    public string CropName { get; set; } = null!;
    public string? Variety { get; set; }
    public DateTime? SowDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public CropSeasonStatus Status { get; set; }
    public decimal? ExpectedYield_kg { get; set; }
    public decimal? ActualYield_kg { get; set; }
}

public class CropSeasonCreateDto
{
    public int LandId { get; set; }
    [Required, MaxLength(100)]
    public string CropName { get; set; } = null!;
    public string? Variety { get; set; }
    public DateTime? SowDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public decimal? ExpectedYield_kg { get; set; }
    public decimal? SeedCost { get; set; }
}

public class CropSeasonEditDto : CropSeasonCreateDto
{
    public int Id { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public decimal? ActualYield_kg { get; set; }
    public CropSeasonStatus Status { get; set; }
}
