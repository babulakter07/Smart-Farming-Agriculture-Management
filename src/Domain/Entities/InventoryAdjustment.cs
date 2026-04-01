using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class InventoryAdjustment : BaseEntity
    {
        public int InventoryId { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public decimal PhysicalCount_kg { get; set; }
        public decimal SystemCount_kg { get; set; }
        public decimal Variance_kg { get; set; }
        public decimal VarianceValue_BDT { get; set; }
        public string Reason { get; set; }
        public int AdjustedBy { get; set; }
        public int ApprovedBy { get; set; }
    }
}