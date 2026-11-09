using Microsoft.AspNetCore.Mvc;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            ViewBag.Products = _context.Products.ToList();

            return View(suppliers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index", "Supplier");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var supllier = _context.Suppliers.Find(Id);
            return View(supllier);
        }

        [HttpPost]

        public IActionResult Edit(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Deteils(int Id)
        {
            var supplier = _context.Suppliers.Find(Id);
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id)
        {
            var deleted = _context.Suppliers.Find(Id);
            if (deleted == null)
            {
                return NotFound();
            }
            _context.Suppliers.Remove(deleted);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

    }
}
