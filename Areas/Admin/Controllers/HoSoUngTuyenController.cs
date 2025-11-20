using System.Collections.Generic;
using System.Linq;
using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoSoUngTuyenController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public HoSoUngTuyenController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var query = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(h => h.TaiKhoan)
                .AsQueryable();
            
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (!employerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                
                var companyId = employerCompanyId.Value;
                query = query.Where(h => h.TinTuyenDung != null && 
                                        h.TinTuyenDung.NhaTuyenDungId == companyId);
            }
            
            var list = query
                .OrderByDescending(h => h.NgayNop)
                .ToList();
            return View(list);
        }
        
        public IActionResult Create()
        {
            if (IsEmployer && !EmployerCompanyId.HasValue)
            {
                TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            
            ViewBag.TinTuyenDungList = BuildTinTuyenDungSelectList();
            
            ViewBag.TaiKhoanList = _context.TaiKhoan
                .Where(t => t.IsActive == true && t.Role == "Candidate")
                .Select(t => new SelectListItem
                {
                    Text = t.HoTen + " (" + t.Email + ")",
                    Value = t.Id.ToString()
                }).ToList();
            
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(HoSoUngTuyen model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayNop = DateTime.Now;
                _context.HoSoUngTuyen.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm hồ sơ ứng tuyển thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.TinTuyenDungList = BuildTinTuyenDungSelectList(model.TinTuyenDungId);
            
            ViewBag.TaiKhoanList = _context.TaiKhoan
                .Where(t => t.IsActive == true && t.Role == "Candidate")
                .Select(t => new SelectListItem
                {
                    Text = t.HoTen + " (" + t.Email + ")",
                    Value = t.Id.ToString()
                }).ToList();
            
            return View(model);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .FirstOrDefault(h => h.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                    return NotFound();
            }
            
            ViewBag.TinTuyenDungList = BuildTinTuyenDungSelectList(model.TinTuyenDungId);
            
            ViewBag.TaiKhoanList = _context.TaiKhoan
                .Where(t => t.IsActive == true && t.Role == "Candidate")
                .Select(t => new SelectListItem
                {
                    Text = t.HoTen + " (" + t.Email + ")",
                    Value = t.Id.ToString(),
                    Selected = t.Id == model.TaiKhoanId
                }).ToList();
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(HoSoUngTuyen model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayCapNhat = DateTime.Now;
                _context.HoSoUngTuyen.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật hồ sơ ứng tuyển thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.TinTuyenDungList = BuildTinTuyenDungSelectList(model.TinTuyenDungId);
            
            ViewBag.TaiKhoanList = _context.TaiKhoan
                .Where(t => t.IsActive == true && t.Role == "Candidate")
                .Select(t => new SelectListItem
                {
                    Text = t.HoTen + " (" + t.Email + ")",
                    Value = t.Id.ToString(),
                    Selected = t.Id == model.TaiKhoanId
                }).ToList();
            
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(h => h.TaiKhoan)
                .Include(h => h.QuaTrinhPhongVans)
                .FirstOrDefault(h => h.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                    return NotFound();
            }
            
            return View(model);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .Include(h => h.TaiKhoan)
                .Include(h => h.QuaTrinhPhongVans)
                .FirstOrDefault(h => h.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                    return NotFound();
            }
            
            ViewBag.HasQuaTrinhPhongVan = model.QuaTrinhPhongVans != null && model.QuaTrinhPhongVans.Any();
            
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .FirstOrDefault(h => h.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsTinTuyenDung(model.TinTuyenDungId))
                    return NotFound();
            }
            
            try
            {
                _context.HoSoUngTuyen.Remove(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa hồ sơ ứng tuyển thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa hồ sơ ứng tuyển này. Có thể đang có quá trình phỏng vấn liên quan.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            return RedirectToAction("Index");
        }
        
        private List<SelectListItem> BuildTinTuyenDungSelectList(int? selectedId = null)
        {
            var query = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .Where(t => t.IsActive == true)
                .AsQueryable();
            
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (employerCompanyId.HasValue)
                {
                    query = query.Where(t => t.NhaTuyenDungId == employerCompanyId.Value);
                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
            
            return query
                .Select(t => new SelectListItem
                {
                    Text = (t.TieuDe ?? string.Empty) + " - " + (t.NhaTuyenDung != null ? t.NhaTuyenDung.TenCongTy : string.Empty),
                    Value = t.Id.ToString(),
                    Selected = selectedId.HasValue && t.Id == selectedId.Value
                })
                .ToList();
        }
        
        private bool EmployerOwnsTinTuyenDung(int tinTuyenDungId)
        {
            var employerCompanyId = EmployerCompanyId;
            if (!employerCompanyId.HasValue)
            {
                return false;
            }
            
            return _context.TinTuyenDung
                .AsNoTracking()
                .Any(t => t.Id == tinTuyenDungId && t.NhaTuyenDungId == employerCompanyId.Value);
        }
    }
}

