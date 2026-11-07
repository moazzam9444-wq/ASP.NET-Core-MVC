
using GraduationSupplyManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GraduationSupplyManagement.Controllers
{
    public class OfferController : Controller
    {
        // Display All Offers
        public IActionResult Index()
        {
            List<SelectedOffer> offers = GetOffers();

            return View(offers);
        }

        // Select Offers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectOffers(
            int? certificateId,
            int? hospitalityId,
            int? giftId)
        {
            if (certificateId == null ||
                hospitalityId == null ||
                giftId == null)
            {
                TempData["Message"] = "Please select one offer from each category.";
                return RedirectToAction("Index");
            }

            List<SelectedOffer> offers = GetOffers();

            SelectedOffer? certificate = offers.FirstOrDefault(
                o => o.Id == certificateId && o.Category == "Graduation Certificates");

            SelectedOffer? hospitality = offers.FirstOrDefault(
                o => o.Id == hospitalityId && o.Category == "Hospitality");

            SelectedOffer? gift = offers.FirstOrDefault(
                o => o.Id == giftId && o.Category == "Graduation Gifts");

            if (certificate == null || hospitality == null || gift == null)
            {
                TempData["Message"] = "Invalid offer selection.";
                return RedirectToAction("Index");
            }

            // Part 1: Separate Session Keys
            HttpContext.Session.SetInt32("CertificateOffer", certificateId.Value);
            HttpContext.Session.SetInt32("HospitalityOffer", hospitalityId.Value);
            HttpContext.Session.SetInt32("GiftOffer", giftId.Value);

            // Part 2: Save List In Session
            List<SelectedOffer> selectedOffers = new List<SelectedOffer>
            {
                certificate,
                hospitality,
                gift
            };

            string json = JsonSerializer.Serialize(selectedOffers);

            HttpContext.Session.SetString("SelectedOffers", json);

            return RedirectToAction("Review");
        }

        // Review Selected Offers
        [HttpGet]
        public IActionResult Review()
        {
            string? json = HttpContext.Session.GetString("SelectedOffers");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            List<SelectedOffer> selectedOffers =
                JsonSerializer.Deserialize<List<SelectedOffer>>(json)!;

            decimal total = 0;
            List<decimal> offerTotals = new List<decimal>();

            foreach (var offer in selectedOffers)
            {
                decimal offerTotal = offer.Quantity * offer.UnitPrice;

                offerTotals.Add(offerTotal);
                total += offerTotal;
            }

            decimal discount = 0;

            if (total > 500)
            {
                discount = 0.15m;
            }
            else if (total > 300)
            {
                discount = 0.10m;
            }

            decimal discountAmount = total * discount;
            decimal finalTotal = total - discountAmount;

            ViewBag.OfferTotals = offerTotals;
            ViewBag.Total = total;
            ViewBag.Discount = discount * 100;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            return View(selectedOffers);
        }

        // About Page
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        // All Available Offers
        private List<SelectedOffer> GetOffers()
        {
            List<SelectedOffer> offers = new List<SelectedOffer>
            {
                new SelectedOffer
                {
                    Id = 1,
                    Category = "Graduation Certificates",
                    SupplierName = "Supplier A",
                    OfferName = "Premium Certificate Package",
                    Quantity = 35,
                    UnitPrice = 2m
                },

                new SelectedOffer
                {
                    Id = 2,
                    Category = "Graduation Certificates",
                    SupplierName = "Supplier B",
                    OfferName = "Standard Certificate Package",
                    Quantity = 35,
                    UnitPrice = 1.5m
                },

                new SelectedOffer
                {
                    Id = 3,
                    Category = "Graduation Certificates",
                    SupplierName = "Supplier C",
                    OfferName = "Luxury Certificate Package",
                    Quantity = 35,
                    UnitPrice = 2.5m
                },

                new SelectedOffer
                {
                    Id = 4,
                    Category = "Hospitality",
                    SupplierName = "Supplier D",
                    OfferName = "Coffee & Drinks Package",
                    Quantity = 100,
                    UnitPrice = 2m
                },

                new SelectedOffer
                {
                    Id = 5,
                    Category = "Hospitality",
                    SupplierName = "Supplier E",
                    OfferName = "Sandwiches & Snacks Package",
                    Quantity = 100,
                    UnitPrice = 3m
                },

                new SelectedOffer
                {
                    Id = 6,
                    Category = "Hospitality",
                    SupplierName = "Supplier F",
                    OfferName = "Full Hospitality Package",
                    Quantity = 100,
                    UnitPrice = 4m
                },

                new SelectedOffer
                {
                    Id = 7,
                    Category = "Graduation Gifts",
                    SupplierName = "Supplier G",
                    OfferName = "Customized Mugs Package",
                    Quantity = 35,
                    UnitPrice = 2m
                },

                new SelectedOffer
                {
                    Id = 8,
                    Category = "Graduation Gifts",
                    SupplierName = "Supplier H",
                    OfferName = "Graduation Bags Package",
                    Quantity = 35,
                    UnitPrice = 3m
                },

                new SelectedOffer
                {
                    Id = 9,
                    Category = "Graduation Gifts",
                    SupplierName = "Supplier I",
                    OfferName = "Premium Gift Boxes Package",
                    Quantity = 35,
                    UnitPrice = 4m
                }
            };

            return offers;
        }
    }
}
