using Microsoft.AspNetCore.Mvc;

namespace UniversityCafeteria.Controllers
{
    public class SupplyController : Controller
    {
        public IActionResult Index()
        {
            List<string> Categories = new List<string>
            {
                "Drinks",
                "Food",
                "Snacks",
                "Hot Drinks",
                "Cold Drinks"
            };
            ViewBag.Categories = Categories;
            return View();
        }
        public IActionResult Products()
        {
            ViewData["Page Title"] = "Cafeteria Products";
            ViewData["Manager Name"] = "Ahmad Ali";
            ViewData["Number of Products"] = 5;
            string[]  products = { "Water", "Orange Juice", "Coffee", "Sandwich", "Chocolate" };
            ViewData["products"]= products;
            return View();
        }
        public IActionResult Suppliers()
        {
            ViewBag.Title = "Our Suppliers";
            ViewBag.NumberOfSuppliers = 3;
            string[] arr = { "Fresh Food Company", "Happy Drinks Company", "Daily Supplies Company" };
            ViewBag.Suppliers = arr;

            return View();
        }
        [HttpGet]
        public IActionResult CreateRequest()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult SubmitRequest()
        {
            string employeeName = Request.Form["EmployeeName"].ToString();
            string productName = Request.Form["ProductName"].ToString();
            string quantity = Request.Form["Quantity"].ToString();
            string requiredDate = Request.Form["RequiredDate"].ToString();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                ViewBag.Message = "Employee Name is required!";
                return View();
            }

            TempData["Message"] = "Request submitted successfully!";

            return RedirectToAction("About");
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
