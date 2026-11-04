using Microsoft.AspNetCore.Mvc;

namespace Session_Cokkies_Test.Controllers
{
    public class CookieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SaveCookie()
        {
            Response.Cookies.Append("Name", "Mohammad");
            return Content("Cookie Saved Successfully");
        }
        public IActionResult Show()
        {
            string StudenName = Request.Cookies["Name"];
            ViewBag.Name = StudenName;
            return View();
        }
    }
}
