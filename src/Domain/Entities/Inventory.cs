using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class Inventory : BaseEntity
    {
        public int FarmId { get; set; }
        public int FeedTypeId { get; set; }
        public string ItemType { get; set; } // Feed / Medicine / Supplement / Equipment / Other
        public decimal CurrentQty_kg { get; set; }
        public decimal WeightedAvgCost { get; set; }
        public decimal? ReorderLevel_kg { get; set; }
        public decimal? MaxCapacity_kg { get; set; }
        public string StorageLocation { get; set; }
        public DateTime? LastUpdated { get; set; }
        // Navigation properties can be added as needed
    }
}