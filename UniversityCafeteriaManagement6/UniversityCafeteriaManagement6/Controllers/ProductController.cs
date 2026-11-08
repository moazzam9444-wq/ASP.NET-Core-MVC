using Microsoft.AspNetCore.Mvc;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();

            ViewBag.Suppliers = _context.Suppliers.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
    }
}
