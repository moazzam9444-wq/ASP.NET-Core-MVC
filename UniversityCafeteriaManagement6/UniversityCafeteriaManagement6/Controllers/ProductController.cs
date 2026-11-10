
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // All logged-in users
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToList();

            return View(products);
        }

        // Admin & SuperAdmin only
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();

            return View();
        }

        // Admin & SuperAdmin only
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Admin & SuperAdmin only
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var product = _context.Products.Find(Id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();

            return View(product);
        }

        // Admin & SuperAdmin only
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            var existing = _context.Products.Find(product.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Quantity = product.Quantity;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // All logged-in users
        [HttpGet]
        public IActionResult Details(int Id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.Id == Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Admin & SuperAdmin only
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id)
        {
            var deleted = _context.Products.Find(Id);

            if (deleted == null)
            {
                return NotFound();
            }

            _context.Products.Remove(deleted);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
