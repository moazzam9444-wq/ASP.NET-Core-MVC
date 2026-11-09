using Microsoft.AspNetCore.Mvc;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var orders= _context.Orders.ToList();
            
            return View(orders);
        }

        [HttpGet]
        
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var order = _context.Orders.Find(Id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Employees = _context.Employees.ToList();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var order = _context.Orders.Find(Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id)
        {
            var deleted = _context.Orders.Find(Id);

            if (deleted == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(deleted);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
