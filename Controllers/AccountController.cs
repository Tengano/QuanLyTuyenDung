using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangChu.Models;

namespace TrangChu.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(DataContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string MatKhau, bool RememberMe = false)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(MatKhau))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", Email);
                
                var taiKhoan = await _context.TaiKhoan
                    .Include(t => t.NhaTuyenDung)
                    .FirstOrDefaultAsync(t => t.Email == Email && t.IsActive == true);

                if (taiKhoan == null)
                {
                    _logger.LogWarning("User not found or inactive: {Email}", Email);
                    TempData["ErrorMessage"] = "Email hoặc mật khẩu không chính xác.";
                    return View();
                }

                if (taiKhoan.MatKhau != MatKhau)
                {
                    _logger.LogWarning("Password mismatch for user: {Email}", Email);
                    TempData["ErrorMessage"] = "Email hoặc mật khẩu không chính xác.";
                    return View();
                }
                
                _logger.LogInformation("Login successful for user: {Email}, Role: {Role}, NhaTuyenDungId: {NhaTuyenDungId}", 
                    Email, taiKhoan.Role, taiKhoan.NhaTuyenDungId);

                HttpContext.Session.SetInt32("UserId", taiKhoan.Id);
                HttpContext.Session.SetString("UserName", taiKhoan.HoTen);
                HttpContext.Session.SetString("UserEmail", taiKhoan.Email);
                HttpContext.Session.SetString("UserRole", taiKhoan.Role);
                
                if (taiKhoan.Role == "Employer")
                {
                    if (taiKhoan.NhaTuyenDungId.HasValue)
                    {
                        HttpContext.Session.SetInt32("EmployerCompanyId", taiKhoan.NhaTuyenDungId.Value);
                        _logger.LogInformation("EmployerCompanyId set to session: {CompanyId}", taiKhoan.NhaTuyenDungId.Value);
                    }
                    else
                    {
                        HttpContext.Session.Remove("EmployerCompanyId");
                        _logger.LogWarning("Employer account {Email} has no NhaTuyenDungId", Email);
                    }
                }
                else
                {
                    HttpContext.Session.Remove("EmployerCompanyId");
                }

                if (taiKhoan.Role == "Admin" || taiKhoan.Role == "Employer")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", Email);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi. Vui lòng thử lại sau.";
                return View();
            }
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(TaiKhoan taiKhoan, string XacNhanMatKhau)
        {
            if (taiKhoan.MatKhau != XacNhanMatKhau)
            {
                ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu xác nhận không khớp.");
            }

            var existingEmail = await _context.TaiKhoan
                .FirstOrDefaultAsync(t => t.Email == taiKhoan.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    taiKhoan.NgayTao = DateTime.Now;
                    taiKhoan.IsActive = true;

                    if (string.IsNullOrEmpty(taiKhoan.Role))
                    {
                        taiKhoan.Role = "Candidate";
                    }

                    _context.TaiKhoan.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration for email: {Email}", taiKhoan.Email);
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi. Vui lòng thử lại sau.";
                }
            }

            return View(taiKhoan);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem hồ sơ.";
                return RedirectToAction("Login");
            }

            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.NhaTuyenDung)
                .FirstOrDefaultAsync(t => t.Id == userId.Value);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản.";
                return RedirectToAction("Login");
            }

            return View(taiKhoan);
        }

        public async Task<IActionResult> Settings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cài đặt tài khoản.";
                return RedirectToAction("Login");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(userId.Value);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản.";
                return RedirectToAction("Login");
            }

            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(TaiKhoan taiKhoan, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            var existingAccount = await _context.TaiKhoan.FindAsync(userId.Value);
            if (existingAccount == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản.";
                return RedirectToAction("Login");
            }

            existingAccount.HoTen = taiKhoan.HoTen;
            existingAccount.SoDienThoai = taiKhoan.SoDienThoai;
            existingAccount.DiaChi = taiKhoan.DiaChi;
            existingAccount.NgayCapNhat = DateTime.Now;

            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (string.IsNullOrEmpty(CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Vui lòng nhập mật khẩu hiện tại.");
                }
                else if (existingAccount.MatKhau != CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                }
                else if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                }
                else if (NewPassword.Length < 6)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu phải có ít nhất 6 ký tự.");
                }
                else
                {
                    existingAccount.MatKhau = NewPassword;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating account settings for user ID: {UserId}", userId);
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi. Vui lòng thử lại sau.";
                }
            }

            return View(existingAccount);
        }
    }
}

