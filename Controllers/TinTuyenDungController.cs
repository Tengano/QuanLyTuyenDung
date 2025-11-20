using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangChu.Models;

namespace TrangChu.Controllers
{
    public class TinTuyenDungController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<TinTuyenDungController> _logger;

        public TinTuyenDungController(DataContext context, ILogger<TinTuyenDungController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(string? search, string? location, string? experience)
        {
            var query = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.TieuDe.Contains(search) || 
                                        (t.MoTaCongViec != null && t.MoTaCongViec.Contains(search)));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(t => t.DiaDiemLamViec != null && t.DiaDiemLamViec.Contains(location));
            }

            if (!string.IsNullOrEmpty(experience))
            {
                query = query.Where(t => t.KinhNghiem != null && t.KinhNghiem.Contains(experience));
            }

            var tinTuyenDungs = query
                .OrderByDescending(t => t.NgayTao)
                .ToList();

            ViewBag.Search = search;
            ViewBag.Location = location;
            ViewBag.Experience = experience;

            return View(tinTuyenDungs);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinTuyenDung = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .FirstOrDefault(t => t.Id == id);

            if (tinTuyenDung == null || !tinTuyenDung.IsActive)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var daUngTuyen = _context.HoSoUngTuyen
                    .Any(h => h.TinTuyenDungId == id && h.TaiKhoanId == userId.Value);
                ViewBag.DaUngTuyen = daUngTuyen;
            }
            else
            {
                ViewBag.DaUngTuyen = false;
            }

            return View(tinTuyenDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(int tinTuyenDungId, string hoTen, string email, string soDienThoai, string cvUrl, string thuXinViec)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để ứng tuyển.";
                return RedirectToAction("Login", "Account");
            }

            var daUngTuyen = _context.HoSoUngTuyen
                .Any(h => h.TinTuyenDungId == tinTuyenDungId && h.TaiKhoanId == userId.Value);

            if (daUngTuyen)
            {
                TempData["ErrorMessage"] = "Bạn đã ứng tuyển cho vị trí này rồi.";
                return RedirectToAction("Details", new { id = tinTuyenDungId });
            }

            var taiKhoan = _context.TaiKhoan.Find(userId.Value);
            if (taiKhoan != null)
            {
                if (string.IsNullOrEmpty(hoTen)) hoTen = taiKhoan.HoTen;
                if (string.IsNullOrEmpty(email)) email = taiKhoan.Email;
                if (string.IsNullOrEmpty(soDienThoai)) soDienThoai = taiKhoan.SoDienThoai;
            }

            var hoSoUngTuyen = new HoSoUngTuyen
            {
                TinTuyenDungId = tinTuyenDungId,
                TaiKhoanId = userId.Value,
                HoTen = hoTen,
                Email = email,
                SoDienThoai = soDienThoai,
                CvUrl = cvUrl,
                ThuXinViec = thuXinViec,
                TrangThai = "Chờ xử lý",
                NgayNop = DateTime.Now
            };

            _context.HoSoUngTuyen.Add(hoSoUngTuyen);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Ứng tuyển thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất có thể.";
            return RedirectToAction("Details", new { id = tinTuyenDungId });
        }
    }
}

