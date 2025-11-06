using TrangChu.Models;
using TrangChu.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly DataContext _context;
        public MenuController(DataContext context)
        {   
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.Menu.OrderBy(m => m.ID).ToList();
            return View(mnList);
        }
    }
}