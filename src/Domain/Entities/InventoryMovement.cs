using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class InventoryMovement : BaseEntity
    {
        public int InventoryId { get; set; }
        public int FarmId { get; set; }
        public int FeedTypeId { get; set; }
        public string MovementType { get; set; } // Purchase / Consumption / TransferOut / TransferIn / Adjustment / Wastage / Opening
        public DateTime MovementDate { get; set; }
        public decimal Qty_kg { get; set; }
        public decimal UnitCost_BDT { get; set; }
        public decimal? QtyBefore_kg { get; set; }
        public decimal? QtyAfter_kg { get; set; }
        public decimal? WACBefore { get; set; }
        public decimal? WACAfter { get; set; }
        public int? LinkedBatchId { get; set; }
        public int? LinkedTransferId { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNo { get; set; }
        public string Notes { get; set; }
        public int? EnteredBy { get; set; }
        public DateTime? EnteredAt { get; set; }
    }
}