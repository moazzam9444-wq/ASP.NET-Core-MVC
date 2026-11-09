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
            var categories = _context.Categories.ToList();
            ViewBag.Products = _context.Products.ToList();

            return View(categories);
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
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var category= _context.Categories.Find(Id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var deleted= _context.Categories.Find(Id);

            if (deleted == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(deleted);
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet] 
        public IActionResult Deteils(int Id)
        {
            var categories = _context.Categories.Find(Id);

            return View(categories);
        }

    }
}
