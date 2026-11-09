namespace UniversityCafeteriaManagement6.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string OrderDate { get; set; }

        public int TotalPrice { get; set; }

        public string status  { get; set; }

        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
