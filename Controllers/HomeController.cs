using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangChu.Models;

namespace TrangChu.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;

    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var tinTuyenDungs = _context.TinTuyenDung
            .Include(t => t.NhaTuyenDung)
            .Where(t => t.IsActive == true)
            .OrderByDescending(t => t.NgayTao)
            .Take(6)
            .ToList();

        ViewBag.TinTuyenDungs = tinTuyenDungs;

        var userId = HttpContext.Session.GetInt32("UserId");
        var userRole = HttpContext.Session.GetString("UserRole");
        
        if (userId.HasValue && userRole == "Candidate")
        {
            var hoSoUngTuyens = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .ThenInclude(t => t!.NhaTuyenDung)
                .Include(h => h.QuaTrinhPhongVans!)
                .ThenInclude(q => q!.KetQuaPhongVan)
                .Where(h => h.TaiKhoanId == userId.Value)
                .OrderByDescending(h => h.NgayNop)
                .Take(5)
                .ToList();

            ViewBag.HoSoUngTuyens = hoSoUngTuyens;

            var quaTrinhPhongVans = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h!.TinTuyenDung)
                .ThenInclude(t => t!.NhaTuyenDung)
                .Include(q => q.KetQuaPhongVan)
                .Where(q => q.HoSoUngTuyen != null && q.HoSoUngTuyen.TaiKhoanId == userId.Value)
                .OrderByDescending(q => q.NgayPhongVan)
                .Take(5)
                .ToList();

            ViewBag.QuaTrinhPhongVans = quaTrinhPhongVans;
        }
        else
        {
            ViewBag.HoSoUngTuyens = new List<HoSoUngTuyen>();
            ViewBag.QuaTrinhPhongVans = new List<QuaTrinhPhongVan>();
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
