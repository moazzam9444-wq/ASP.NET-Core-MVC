namespace UniversityCafeteriaManagement6.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }
        public string? Description { get; set; }

        public Category Category { get; set; }
        public Supplier Supplier { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }
    }
    //CategoryId, SupplierId
}
