using Microsoft.AspNetCore.Mvc;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var category = _context.Categories.ToList();

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index","Category");
        }
    }
}
