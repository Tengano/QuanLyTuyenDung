using System.Collections.Generic;
using System.Linq;
using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TinTuyenDungController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public TinTuyenDungController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var query = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
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
                query = query.Where(t => t.NhaTuyenDungId == companyId);
            }
            
            var list = query
                .OrderByDescending(t => t.NgayTao)
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
            
            ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList();
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(TinTuyenDung model)
        {
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (!employerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                
                model.NhaTuyenDungId = employerCompanyId.Value;
            }
            
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                _context.TinTuyenDung.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm tin tuyển dụng thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList(model.NhaTuyenDungId);
            return View(model);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var companyId = EmployerCompanyId.Value;
                if (model.NhaTuyenDungId != companyId)
                    return NotFound();
            }
            
            ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList(model.NhaTuyenDungId);
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(TinTuyenDung model)
        {
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (!employerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                
                model.NhaTuyenDungId = employerCompanyId.Value;
            }
            
            if (ModelState.IsValid)
            {
                if (IsEmployer)
                {
                    var existing = _context.TinTuyenDung
                        .AsNoTracking()
                        .FirstOrDefault(t => t.Id == model.Id);
                    if (existing == null || existing.NhaTuyenDungId != model.NhaTuyenDungId)
                    {
                        return NotFound();
                    }
                }
                
                model.NgayCapNhat = DateTime.Now;
                _context.TinTuyenDung.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật tin tuyển dụng thành công.";
                return RedirectToAction("Index");
            }
            
            ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList(model.NhaTuyenDungId);
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .Include(t => t.HoSoUngTuyens)
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var companyId = EmployerCompanyId.Value;
                if (model.NhaTuyenDungId != companyId)
                    return NotFound();
            }
            
            return View(model);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.TinTuyenDung
                .Include(t => t.NhaTuyenDung)
                .Include(t => t.HoSoUngTuyens)
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var companyId = EmployerCompanyId.Value;
                if (model.NhaTuyenDungId != companyId)
                    return NotFound();
            }
            
            ViewBag.HasHoSoUngTuyen = model.HoSoUngTuyens != null && model.HoSoUngTuyens.Any();
            
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.TinTuyenDung.Find(id);
            if (model == null)
                return NotFound();
            
            if (IsEmployer)
            {
                if (!EmployerCompanyId.HasValue)
                {
                    TempData["ErrorMessage"] = "Không xác định được công ty của nhà tuyển dụng. Vui lòng liên hệ quản trị viên để được gán công ty.";
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                var companyId = EmployerCompanyId.Value;
                if (model.NhaTuyenDungId != companyId)
                    return NotFound();
            }
            
            try
            {
                _context.TinTuyenDung.Remove(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa tin tuyển dụng thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa tin tuyển dụng này. Có thể đang có hồ sơ ứng tuyển liên quan.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            return RedirectToAction("Index");
        }
        
        private List<SelectListItem> BuildNhaTuyenDungSelectList(int? selectedId = null)
        {
            var query = _context.NhaTuyenDung
                .Where(n => n.IsActive == true)
                .AsQueryable();
            
            if (IsEmployer)
            {
                var employerCompanyId = EmployerCompanyId;
                if (employerCompanyId.HasValue)
                {
                    query = query.Where(n => n.Id == employerCompanyId.Value);
                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
            
            return query
                .Select(n => new SelectListItem
                {
                    Text = n.TenCongTy,
                    Value = n.Id.ToString(),
                    Selected = selectedId.HasValue && n.Id == selectedId.Value
                })
                .ToList();
        }
    }
}

