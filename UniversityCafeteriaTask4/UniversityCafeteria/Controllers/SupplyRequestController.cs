using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityCafeteria.Models;
using System.Text.Json;
namespace UniversityCafeteria.Controllers
{
    public class SupplyRequestController : Controller
    {
        private static readonly List<SupplyRequest> ConfirmedRequests = new();

        private static readonly object RequestsLock = new();
        private static int NextRequestId = 1;
        [HttpGet]
        public IActionResult Index()
        {
            string? preferredPriority = Request.Cookies["PreferredPriority"];

            ViewBag.PreferredPriority =
                preferredPriority is "Normal" or "Urgent" or "Emergency"
                ? preferredPriority
                : "Normal";

            return View();
        }

        [HttpPost]
        public IActionResult AddItem(SupplyItem item)
        {
            if (item.Quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than zero";

                return RedirectToAction("Index");
            }
            if (item.UnitPrice <= 0) {

                TempData["Error2"] = "UnitPrice Must be greater than zero";
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                TempData["Error3"] = "ProductName Must Not Be Null";
                return RedirectToAction("Index");
            }
            List<SupplyItem> SupplyItems = new List<SupplyItem>();

            string? oldJson = HttpContext.Session.GetString("SupplyItems");

            if (!string.IsNullOrEmpty(oldJson))
            {
                SupplyItems = JsonSerializer.Deserialize<List<SupplyItem>>(oldJson)!;
            }

            SupplyItems.Add(item);

            string json = JsonSerializer.Serialize(SupplyItems);

            HttpContext.Session.SetString("SupplyItems", json);

        
            return RedirectToAction("Index");
        }



        public IActionResult Review()
        {
            // 1. Create empty list
            List<SupplyItem> SupplyItems = new List<SupplyItem>();

            // 2. Get products from Session
            string? json = HttpContext.Session.GetString("SupplyItems");

            if (!string.IsNullOrEmpty(json))
            {
                SupplyItems = JsonSerializer.Deserialize<List<SupplyItem>>(json)
                              ?? new List<SupplyItem>();
            }

            // 3. Calculate Subtotal
            decimal subtotal = 0;

            foreach (SupplyItem item in SupplyItems)
            {
                decimal total = item.Quantity * item.UnitPrice;
                subtotal += total;
            }

            // 4. Calculate Priority Fee
            decimal priorityFee = 0;

            if (SupplyItems.Count > 0)
            {
                string priority = SupplyItems[0].Priority;

                switch (priority)
                {
                    case "Normal":
                        priorityFee = 2;
                        break;

                    case "Urgent":
                        priorityFee = 5;
                        break;

                    case "Emergency":
                        priorityFee = 10;
                        break;
                }
            }

            // 5. Calculate Discount
            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            // 6. Calculate Final Total
            decimal finalTotal = subtotal + priorityFee - discount;

            // 7. Send results to View
            ViewBag.Subtotal = subtotal;
            ViewBag.PriorityFee = priorityFee;
            ViewBag.Discount = discount;
            ViewBag.FinalTotal = finalTotal;

            return View(SupplyItems);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRequest()
        {
            // 1. Read draft items from Session
            string? json = HttpContext.Session.GetString("SupplyItems");

            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "Cannot confirm an empty request.";
                return RedirectToAction("Review");
            }

            List<SupplyItem> items =
                JsonSerializer.Deserialize<List<SupplyItem>>(json)
                ?? new List<SupplyItem>();

            // 2. Prevent empty requests
            if (items.Count == 0)
            {
                TempData["Error"] = "Cannot confirm an empty request.";
                return RedirectToAction("Review");
            }

            // 3. Calculate subtotal
            decimal subtotal = 0;

            foreach (SupplyItem item in items)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }

            // 4. Calculate priority fee
            decimal priorityFee = 0;

            string priority = items[0].Priority;

            switch (priority)
            {
                case "Normal":
                    priorityFee = 2;
                    break;

                case "Urgent":
                    priorityFee = 5;
                    break;

                case "Emergency":
                    priorityFee = 10;
                    break;
            }

            // 5. Calculate discount
            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            // 6. Calculate final total
            decimal finalTotal = subtotal + priorityFee - discount;

            // 7. Create confirmed request
            SupplyRequest request = new SupplyRequest
            {
                ProductCount = items.Count,
                Subtotal = subtotal,
                PriorityFee = priorityFee,
                Discount = discount,
                FinalTotal = finalTotal,
                Status = "Confirmed"
            };

            // 8. Save request in application list
            lock (RequestsLock)
            {
                request.RequestId = NextRequestId++;
                ConfirmedRequests.Add(request);
            }

            // 9. Clear draft Session
            HttpContext.Session.Remove("SupplyItems");

            TempData["Success"] = "Request confirmed successfully!";

            // 10. Redirect to MyRequests
            return RedirectToAction("MyRequests");
        }
        [HttpPost]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove("SupplyItems");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SavePreference(string Priority)
        {
            if (Priority != "Normal" &&
                Priority != "Urgent" &&
                Priority != "Emergency")
            {
                return BadRequest("Invalid Priority");
            }

            CookieOptions options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("PreferredPriority", Priority, options);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult MyRequests()
        {
            List<SupplyRequest> requests;

            lock (RequestsLock)
            {
                requests = ConfirmedRequests.ToList();
            }

            return View(requests);
        }
        public IActionResult About()
        {
            return View();
        }


    }
}
