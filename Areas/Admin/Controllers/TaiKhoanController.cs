using System.Collections.Generic;
using System.Linq;
using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public TaiKhoanController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(string searchString, string filterRole)
        {
            var query = _context.TaiKhoan
                .Include(t => t.NhaTuyenDung)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => 
                    t.HoTen.Contains(searchString) ||
                    t.Email.Contains(searchString) ||
                    (t.SoDienThoai != null && t.SoDienThoai.Contains(searchString))
                );
            }
            
            if (!string.IsNullOrEmpty(filterRole))
            {
                query = query.Where(t => t.Role == filterRole);
            }
            
            var list = query.OrderByDescending(t => t.NgayTao).ToList();
            
            ViewBag.SearchString = searchString;
            ViewBag.FilterRole = filterRole;
            
            return View(list);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.TaiKhoan
                .Include(t => t.NhaTuyenDung)
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
                return NotFound();
            
            if (model.Role == "Employer")
            {
                ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList(model.NhaTuyenDungId);
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaiKhoan model)
        {
            var existingModel = _context.TaiKhoan.Find(model.Id);
            if (existingModel == null)
                return NotFound();
            
            ModelState.Remove("MatKhau");
            ModelState.Remove("XacNhanMatKhau");
            
            if (_context.TaiKhoan.Any(t => t.Email == model.Email && t.Id != model.Id))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác.");
            }
            
            if (model.Role == "Employer" && model.NhaTuyenDungId.HasValue)
            {
                if (!_context.NhaTuyenDung.Any(n => n.Id == model.NhaTuyenDungId.Value))
                {
                    ModelState.AddModelError("NhaTuyenDungId", "Công ty không tồn tại.");
                }
            }
            
            if (ModelState.IsValid)
            {
                existingModel.HoTen = model.HoTen;
                existingModel.Email = model.Email;
                existingModel.SoDienThoai = model.SoDienThoai;
                existingModel.DiaChi = model.DiaChi;
                existingModel.Role = model.Role;
                existingModel.IsActive = model.IsActive;
                
                existingModel.NhaTuyenDungId = model.NhaTuyenDungId;
                
                if (model.Role != "Employer")
                {
                    existingModel.NhaTuyenDungId = null;
                }
                
                existingModel.NgayCapNhat = DateTime.Now;
                
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction("Index");
            }
            
            model.MatKhau = existingModel.MatKhau;
            if (model.Role == "Employer")
            {
                ViewBag.NhaTuyenDungList = BuildNhaTuyenDungSelectList(model.NhaTuyenDungId);
            }
            
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.TaiKhoan
                .Include(t => t.NhaTuyenDung)
                .FirstOrDefault(t => t.Id == id);
            if (model == null)
                return NotFound();
            
            return View(model);
        }
        
        private List<SelectListItem> BuildNhaTuyenDungSelectList(int? selectedId = null)
        {
            return _context.NhaTuyenDung
                .Where(n => n.IsActive == true)
                .OrderBy(n => n.TenCongTy)
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

