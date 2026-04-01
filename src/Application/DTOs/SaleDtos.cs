using System.ComponentModel.DataAnnotations;

namespace Firming_Solution.Application.DTOs;

public class SaleListDto
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public string BatchName { get; set; } = null!;
    public string? BuyerName { get; set; }
    public DateTime SaleDate { get; set; }
    public int? Quantity { get; set; }
    public decimal? TotalWeight_kg { get; set; }
    public decimal PricePerKg { get; set; }
    public decimal TotalRevenue { get; set; }
    public bool IsEidSale { get; set; }
}

public class SaleCreateDto
{
    public int BatchId { get; set; }
    public int? BuyerId { get; set; }
    [Required]
    public DateTime SaleDate { get; set; }
    public int? Quantity { get; set; }
    public decimal? TotalWeight_kg { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal PricePerKg { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal TotalRevenue { get; set; }
    public bool IsEidSale { get; set; }
    public string? Notes { get; set; }
}

public class SaleEditDto : SaleCreateDto
{
    public int Id { get; set; }
}
