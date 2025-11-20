using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuaTrinhPhongVanController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public QuaTrinhPhongVanController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var query = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TaiKhoan)
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
                query = query.Where(q => q.HoSoUngTuyen != null && 
                                       q.HoSoUngTuyen.TinTuyenDung != null &&
                                       q.HoSoUngTuyen.TinTuyenDung.NhaTuyenDungId == companyId);
            }
            
            var list = query
                .OrderByDescending(q => q.NgayPhongVan)
                .ToList();
            return View(list);
        }
        
        public IActionResult Create(int? hoSoUngTuyenId)
        {
            if (IsEmployer && !EmployerCompanyId.HasValue)
            {
                TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng.";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            
            ViewBag.HoSoUngTuyenList = BuildHoSoUngTuyenSelectList(
                h => h.TrangThai == "Đã duyệt" || h.TrangThai == "Chờ xử lý" || (hoSoUngTuyenId.HasValue && h.Id == hoSoUngTuyenId.Value),
                hoSoUngTuyenId);
            
            if (hoSoUngTuyenId.HasValue)
            {
                ViewBag.HoSoUngTuyenId = hoSoUngTuyenId.Value;
            }
            
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(QuaTrinhPhongVan model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                _context.QuaTrinhPhongVan.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm quá trình phỏng vấn thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.HoSoUngTuyenList = BuildHoSoUngTuyenSelectList(
                h => h.TrangThai == "Đã duyệt" || h.TrangThai == "Chờ xử lý");
            
            return View(model);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .FirstOrDefault(q => q.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                    return NotFound();
            }
            
            ViewBag.HoSoUngTuyenList = BuildHoSoUngTuyenSelectList(
                h => h.TrangThai == "Đã duyệt" || h.TrangThai == "Chờ xử lý" || h.Id == model.HoSoUngTuyenId,
                model.HoSoUngTuyenId);
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(QuaTrinhPhongVan model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayCapNhat = DateTime.Now;
                _context.QuaTrinhPhongVan.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật quá trình phỏng vấn thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.HoSoUngTuyenList = BuildHoSoUngTuyenSelectList(
                h => h.TrangThai == "Đã duyệt" || h.TrangThai == "Chờ xử lý" || h.Id == model.HoSoUngTuyenId,
                model.HoSoUngTuyenId);
            
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TaiKhoan)
                .Include(q => q.KetQuaPhongVan)
                .FirstOrDefault(q => q.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                    return NotFound();
            }
            
            return View(model);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .Include(q => q.KetQuaPhongVan)
                .FirstOrDefault(q => q.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                    return NotFound();
            }
            
            ViewBag.HasKetQua = model.KetQuaPhongVan != null;
            
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .FirstOrDefault(q => q.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsHoSoUngTuyen(model.HoSoUngTuyenId))
                    return NotFound();
            }
            
            try
            {
                _context.QuaTrinhPhongVan.Remove(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa quá trình phỏng vấn thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa quá trình phỏng vấn này. Có thể đang có kết quả phỏng vấn liên quan.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            return RedirectToAction("Index");
        }
        
        private List<SelectListItem> BuildHoSoUngTuyenSelectList(System.Linq.Expressions.Expression<Func<HoSoUngTuyen, bool>> filter, int? selectedId = null)
        {
            var query = _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(h => h.TaiKhoan)
                .Where(filter)
                .AsQueryable();
            
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (!employerCompanyId.HasValue)
                {
                    return new List<SelectListItem>();
                }
                
                var employerCompanyIdValue = employerCompanyId.Value;
                query = query.Where(h => h.TinTuyenDung!.NhaTuyenDungId == employerCompanyIdValue);
            }
            
            return query
                .AsEnumerable()
                .Select(h => new SelectListItem
                {
                    Text = $"{h.TaiKhoan?.HoTen ?? "-"} - {h.TinTuyenDung?.TieuDe ?? "-"} ({h.TinTuyenDung?.NhaTuyenDung?.TenCongTy ?? "-"})",
                    Value = h.Id.ToString(),
                    Selected = selectedId.HasValue && h.Id == selectedId.Value
                })
                .ToList();
        }
        
        private bool EmployerOwnsHoSoUngTuyen(int hoSoUngTuyenId)
        {
            var employerCompanyId = EmployerCompanyId;
            if (!employerCompanyId.HasValue)
            {
                return false;
            }
            
            return _context.HoSoUngTuyen
                .Include(h => h.TinTuyenDung)
                .AsNoTracking()
                .Any(h => h.Id == hoSoUngTuyenId &&
                          h.TinTuyenDung!.NhaTuyenDungId == employerCompanyId.Value);
        }
    }
}

