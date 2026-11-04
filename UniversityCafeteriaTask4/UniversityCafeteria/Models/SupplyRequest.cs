namespace UniversityCafeteria.Models
{
    public class SupplyRequest
    {
        public int RequestId { get; set; }
        public int ProductCount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PriorityFee { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalTotal { get; set; }
        public string Status { get; set; } = "Confirmed";
    }
}