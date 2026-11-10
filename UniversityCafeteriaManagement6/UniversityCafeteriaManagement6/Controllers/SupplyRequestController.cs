
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaManagement6.Data;
using UniversityCafeteriaManagement6.Models;

namespace UniversityCafeteriaManagement6.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplyRequestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Current user's requests
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var requests = await _context.SupplyRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // Create GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                ModelState.AddModelError("", "Product not found.");
            else if (quantity <= 0)
                ModelState.AddModelError("", "Quantity must be greater than zero.");
            else if (quantity > product.Quantity)
                ModelState.AddModelError("", "Quantity exceeds available stock.");
            else if (product.Price < 0)
                ModelState.AddModelError("", "Invalid product price.");

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _context.Products.ToListAsync();
                return View();
            }

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var request = new SupplyRequest
            {
                UserId = userId,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                TotalPrice = (decimal)product!.Price * quantity,

                RequestItems = new List<RequestItem>
                {
                    new RequestItem
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = product.Price
                    }
                }
            };

            _context.SupplyRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.User)
                .Include(r => r.RequestItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            bool isAdmin = User.IsInRole("Admin")
                || User.IsInRole("SuperAdmin");

            if (request.UserId != userId && !isAdmin)
                return Forbid();

            return View(request);
        }

        // Edit GET - owner only, Pending only
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.RequestItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            if (request.UserId != _userManager.GetUserId(User))
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Only pending requests can be edited.");

            var item = request.RequestItems.SingleOrDefault();

            if (item == null)
                return BadRequest("This request must contain exactly one item.");

            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.ProductId = item.ProductId;
            ViewBag.Quantity = item.Quantity;

            return View(request);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, int productId, int quantity)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.RequestItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            if (request.UserId != _userManager.GetUserId(User))
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Only pending requests can be edited.");

            var item = request.RequestItems.SingleOrDefault();

            if (item == null)
                return BadRequest("This request must contain exactly one item.");

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                ModelState.AddModelError("", "Product not found.");
            else if (quantity <= 0)
                ModelState.AddModelError("", "Quantity must be greater than zero.");
            else if (quantity > product.Quantity)
                ModelState.AddModelError("", "Quantity exceeds available stock.");
            else if (product.Price < 0)
                ModelState.AddModelError("", "Invalid product price.");

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _context.Products.ToListAsync();
                ViewBag.ProductId = productId;
                ViewBag.Quantity = quantity;
                return View(request);
            }

            item.ProductId = product!.Id;
            item.Quantity = quantity;
            item.Price = product.Price;

            request.TotalPrice = (decimal)product.Price * quantity;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete POST - owner only, Pending only
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.RequestItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            if (request.UserId != _userManager.GetUserId(User))
                return Forbid();

            if (request.Status != "Pending")
                return BadRequest("Only pending requests can be deleted.");

            _context.RequestItems.RemoveRange(request.RequestItems);
            _context.SupplyRequests.Remove(request);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // All requests - Admin and SuperAdmin
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> AllRequests()
        {
            var requests = await _context.SupplyRequests
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // Approve
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.SupplyRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending.");

            request.Status = "Approved";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllRequests));
        }

        // Reject
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.SupplyRequests.FindAsync(id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending.");

            request.Status = "Rejected";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllRequests));
        }
    }
}
