using Microsoft.AspNetCore.Mvc;

namespace TrangChu.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}


