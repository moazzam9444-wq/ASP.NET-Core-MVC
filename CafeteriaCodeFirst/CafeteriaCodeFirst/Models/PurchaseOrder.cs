namespace CafeteriaCodeFirst.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public Supplier Supplier { get; set; }
        public int SupplierId { get; set; }

        public List<PurchaseOrderItem> Items { get; set; }

    }
}
//Id, OrderDate, Status, SupplierId
