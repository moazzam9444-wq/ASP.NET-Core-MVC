namespace CafeteriaCodeFirst.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        
        public Supplier Supplier { get; set; }

        public int SupplierId { get; set; }

        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; } 
    }
}
//Id, Name, Price, Quantity, CategoryId, SupplierId
