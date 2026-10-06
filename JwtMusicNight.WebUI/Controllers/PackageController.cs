using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("SignIn", "Login");

            ViewBag.PackageName = HttpContext.Session.GetString("PackageName") ?? "Free";
            ViewBag.PackageLevel = HttpContext.Session.GetInt32("PackageLevel") ?? 0;
            ViewBag.IsAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
            return View();
        }
    }
}
