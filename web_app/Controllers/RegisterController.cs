using Microsoft.AspNetCore.Mvc;

namespace web_app.Controllers;

public class RegisterController : Controller
{
    // GET
    public IActionResult Index()
    {
         return View();
    }
}