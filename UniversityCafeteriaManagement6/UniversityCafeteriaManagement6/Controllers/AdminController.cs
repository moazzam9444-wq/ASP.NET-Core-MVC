
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaManagement6.Data;

namespace UniversityCafeteriaManagement6.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // Display all registered users
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var roles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                roles[user.Id] = string.Join(", ", userRoles);
            }

            ViewBag.UserRoles = roles;

            return View(users);
        }

        // Edit user role GET
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.CurrentRole = roles.Contains("SuperAdmin")
                ? "SuperAdmin"
                : roles.Contains("Admin") ? "Admin" : "User";

            return View(user);
        }

        // Edit user role POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string role)
        {
            string[] allowedRoles = { "User", "Admin", "SuperAdmin" };

            if (!allowedRoles.Contains(role))
                return BadRequest("Invalid role.");

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Do not change your own administrative role
            if (user.Id == _userManager.GetUserId(User))
                return BadRequest("You cannot change your own role.");

            // Keep at least one SuperAdmin
            if (currentRoles.Contains("SuperAdmin") && role != "SuperAdmin")
            {
                var superAdmins = await _userManager
                    .GetUsersInRoleAsync("SuperAdmin");

                if (superAdmins.Count <= 1)
                    return BadRequest("At least one SuperAdmin is required.");
            }

            // No changes needed
            if (currentRoles.Count == 1 && currentRoles.Contains(role))
                return RedirectToAction(nameof(Index));

            var removeResult = await _userManager
                .RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.CurrentRole = currentRoles.FirstOrDefault() ?? "User";
                return View(user);
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);

            if (!addResult.Succeeded)
            {
                // Try to restore previous roles if assignment fails
                await _userManager.AddToRolesAsync(user, currentRoles);

                foreach (var error in addResult.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.CurrentRole = currentRoles.FirstOrDefault() ?? "User";
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }

        // Delete user POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Prevent deleting your own account
            if (user.Id == _userManager.GetUserId(User))
                return BadRequest("You cannot delete your own account.");

            // Protect the last SuperAdmin
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                var superAdmins = await _userManager
                    .GetUsersInRoleAsync("SuperAdmin");

                if (superAdmins.Count <= 1)
                    return BadRequest("Cannot delete the last SuperAdmin.");
            }

            // Keep existing supply request history
            // rather than deleting it accidentally

            var hasRequests = await _context.SupplyRequests
                .AnyAsync(r => r.UserId == id);


            if (hasRequests)
            {
                TempData["Error"] =
                    "This user has supply requests and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
