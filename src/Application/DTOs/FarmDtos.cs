using System.ComponentModel.DataAnnotations;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Application.DTOs;

public class FarmListDto
{
    public int Id { get; set; }
    public string FarmName { get; set; } = null!;
    public FarmType FarmType { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public int BatchCount { get; set; }
    public decimal? TotalArea { get; set; }
}

public class FarmCreateDto
{
    [Required, MaxLength(150)]
    public string FarmName { get; set; } = null!;
    public FarmType FarmType { get; set; }
    [MaxLength(300)]
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? TotalArea { get; set; }
}

public class FarmEditDto : FarmCreateDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class FarmDetailDto : FarmEditDto
{
    public string OwnerName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public List<BatchListDto> Batches { get; set; } = [];
}
