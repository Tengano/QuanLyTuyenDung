using Microsoft.AspNetCore.Mvc;

namespace TrangChu.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string name, string email, string subject, string message)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Tin nhắn của bạn đã được gửi thành công. Chúng tôi sẽ phản hồi sớm nhất có thể!";
                return RedirectToAction("Index");
            }
            
            return View();
        }
    }
}

