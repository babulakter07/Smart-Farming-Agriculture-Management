using System;
using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities
{
    public class InventoryPurchase : BaseEntity
    {
        public int FarmId { get; set; }
        public int FeedTypeId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Qty_kg { get; set; }
        public decimal UnitPrice_BDT { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; } // Paid / Credit / Partial
        public DateTime? PaymentDueDate { get; set; }
        public int? EnteredBy { get; set; }
    }
}