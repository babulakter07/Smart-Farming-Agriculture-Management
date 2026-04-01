using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class FeedTransfer : BaseEntity
    {
        public int FromFarmId { get; set; }
        public int ToFarmId { get; set; }
        public int FeedTypeId { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal Qty_kg { get; set; }
        public decimal? TransportCost { get; set; }
        public string TransportCostAllocation { get; set; } // Source / Destination / Split
        public string Status { get; set; } // Pending / InTransit / Received / Cancelled
        public int RequestedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public int? ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal? ReceivedQty_kg { get; set; }
        public string Notes { get; set; }
    }
}