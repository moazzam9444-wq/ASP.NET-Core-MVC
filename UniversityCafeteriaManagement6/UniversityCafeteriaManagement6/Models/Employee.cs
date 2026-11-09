namespace UniversityCafeteriaManagement6.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }

        public DateTime HireDate { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();

        public  decimal Salary { get; set; }
    }
}
