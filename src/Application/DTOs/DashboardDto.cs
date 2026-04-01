namespace Firming_Solution.Application.DTOs;

public class DashboardDto
{
    public int TotalFarms { get; set; }
    public int ActiveBatches { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal NetProfit { get; set; }
    public decimal TotalCosts { get; set; }
    public int PendingTasks { get; set; }
    public int EidTargetBatches { get; set; }
    public List<BatchListDto> RecentBatches { get; set; } = [];
    public List<CostListDto> RecentCosts { get; set; } = [];
    public List<SaleListDto> RecentSales { get; set; } = [];
    public List<string> WeatherAlerts { get; set; } = [];
}
