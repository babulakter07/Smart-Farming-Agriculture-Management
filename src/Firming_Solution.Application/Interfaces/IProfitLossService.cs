namespace Firming_Solution.Application.Interfaces;

public record BatchPLSummary(
    int BatchId,
    string BatchName,
    string Species,
    decimal PurchaseCost,
    decimal TotalFeedCost,
    decimal TotalMedicineCost,
    decimal TotalLabourCost,
    decimal TotalOtherCost,
    decimal TotalRevenue,
    decimal GrossProfit,
    decimal ROI_Pct,
    int InitialCount,
    int LiveCount,
    decimal CostPerHead,
    decimal? BreakevenPricePerKg
);

public interface IProfitLossService
{
    Task<BatchPLSummary?> GetBatchPLAsync(int batchId, CancellationToken ct = default);
    Task<IList<BatchPLSummary>> GetFarmPLAsync(int farmId, CancellationToken ct = default);
}
