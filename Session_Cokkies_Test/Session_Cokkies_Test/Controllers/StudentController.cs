using Microsoft.AspNetCore.Mvc;

namespace Session_Cokkies_Test.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            HttpContext.Session.SetString("Name", "Mohammad");
            return Content("NamedSavedSuccessfully");
        }
        public IActionResult Show()
        {
            string Name = HttpContext.Session.GetString("Name");
            ViewBag.StudentName = Name;
            return View();

        }

    }
}
