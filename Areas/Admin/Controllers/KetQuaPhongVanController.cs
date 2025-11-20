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
    public class KetQuaPhongVanController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public KetQuaPhongVanController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var query = _context.KetQuaPhongVan
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
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
                query = query.Where(k => k.QuaTrinhPhongVan != null &&
                                        k.QuaTrinhPhongVan.HoSoUngTuyen != null &&
                                        k.QuaTrinhPhongVan.HoSoUngTuyen.TinTuyenDung != null &&
                                        k.QuaTrinhPhongVan.HoSoUngTuyen.TinTuyenDung.NhaTuyenDungId == companyId);
            }
            
            var list = query
                .OrderByDescending(k => k.NgayTao)
                .ToList();
            return View(list);
        }
        
        public IActionResult Create()
        {
            if (IsEmployer && !EmployerCompanyId.HasValue)
            {
                TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng.";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            
            ViewBag.QuaTrinhPhongVanList = BuildQuaTrinhPhongVanSelectList(
                q => q.TrangThai == "Đã hoàn thành" && q.KetQuaPhongVan == null);
            
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(KetQuaPhongVan model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                _context.KetQuaPhongVan.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm kết quả phỏng vấn thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.QuaTrinhPhongVanList = BuildQuaTrinhPhongVanSelectList(
                q => q.TrangThai == "Đã hoàn thành" && q.KetQuaPhongVan == null);
            
            return View(model);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.KetQuaPhongVan
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .FirstOrDefault(k => k.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                    return NotFound();
            }
            
            ViewBag.QuaTrinhPhongVanList = BuildQuaTrinhPhongVanSelectList(
                q => q.Id == model.QuaTrinhPhongVanId || (q.TrangThai == "Đã hoàn thành" && q.KetQuaPhongVan == null),
                model.QuaTrinhPhongVanId);
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(KetQuaPhongVan model)
        {
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                {
                    return NotFound();
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayCapNhat = DateTime.Now;
                _context.KetQuaPhongVan.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật kết quả phỏng vấn thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.QuaTrinhPhongVanList = BuildQuaTrinhPhongVanSelectList(
                q => q.Id == model.QuaTrinhPhongVanId || (q.TrangThai == "Đã hoàn thành" && q.KetQuaPhongVan == null),
                model.QuaTrinhPhongVanId);
            
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.KetQuaPhongVan
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TaiKhoan)
                .FirstOrDefault(k => k.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                    return NotFound();
            }
            
            return View(model);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.KetQuaPhongVan
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .FirstOrDefault(k => k.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                    return NotFound();
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.KetQuaPhongVan
                .Include(k => k.QuaTrinhPhongVan)
                .ThenInclude(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .FirstOrDefault(k => k.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                if (!EmployerOwnsQuaTrinhPhongVan(model.QuaTrinhPhongVanId))
                    return NotFound();
            }
            
            try
            {
                _context.KetQuaPhongVan.Remove(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa kết quả phỏng vấn thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa kết quả phỏng vấn này.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            return RedirectToAction("Index");
        }
        
        private List<SelectListItem> BuildQuaTrinhPhongVanSelectList(Expression<Func<QuaTrinhPhongVan, bool>> filter, int? selectedId = null)
        {
            var query = _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .ThenInclude(t => t.NhaTuyenDung)
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TaiKhoan)
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
                query = query.Where(q => q.HoSoUngTuyen!.TinTuyenDung!.NhaTuyenDungId == employerCompanyIdValue);
            }
            
            return query
                .AsEnumerable()
                .Select(q => new SelectListItem
                {
                    Text = $"{q.HoSoUngTuyen?.TaiKhoan?.HoTen ?? "-"} - {q.HoSoUngTuyen?.TinTuyenDung?.TieuDe ?? "-"} ({q.NgayPhongVan:dd/MM/yyyy})",
                    Value = q.Id.ToString(),
                    Selected = selectedId.HasValue && q.Id == selectedId.Value
                })
                .ToList();
        }
        
        private bool EmployerOwnsQuaTrinhPhongVan(int quaTrinhPhongVanId)
        {
            var employerCompanyId = EmployerCompanyId;
            if (!employerCompanyId.HasValue)
            {
                return false;
            }
            
            return _context.QuaTrinhPhongVan
                .Include(q => q.HoSoUngTuyen)
                .ThenInclude(h => h.TinTuyenDung)
                .AsNoTracking()
                .Any(q => q.Id == quaTrinhPhongVanId &&
                          q.HoSoUngTuyen!.TinTuyenDung!.NhaTuyenDungId == employerCompanyId.Value);
        }
    }
}

