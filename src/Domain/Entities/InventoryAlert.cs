using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class InventoryAlert : BaseEntity
    {
        public int InventoryId { get; set; }
        public int FarmId { get; set; }
        public int FeedTypeId { get; set; }
        public string AlertType { get; set; } // LowStock / OutOfStock / ExcessStock / TransferRequired
        public DateTime AlertDate { get; set; }
        public decimal CurrentQty_kg { get; set; }
        public decimal ReorderLevel_kg { get; set; }
        public decimal? DaysStockRemaining { get; set; }
        public bool IsAcknowledged { get; set; }
        public int? AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public bool NotificationSent { get; set; }
    }
}