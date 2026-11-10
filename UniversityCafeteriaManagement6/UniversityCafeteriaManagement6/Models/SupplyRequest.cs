using UniversityCafeteriaManagement6.Data;

namespace UniversityCafeteriaManagement6.Models
{
    public class SupplyRequest
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public List<RequestItem> RequestItems { get; set; } = new();
    }
}