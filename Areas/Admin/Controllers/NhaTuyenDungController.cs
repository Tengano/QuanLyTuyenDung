using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhaTuyenDungController : BaseAdminController
    {
        private readonly DataContext _context;
        
        public NhaTuyenDungController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(string searchString, string filterStatus)
        {
            var query = _context.NhaTuyenDung.AsQueryable();
            

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => 
                    n.TenCongTy.Contains(searchString) ||
                    (n.Email != null && n.Email.Contains(searchString)) ||
                    (n.SoDienThoai != null && n.SoDienThoai.Contains(searchString)) ||
                    (n.MaSoThue != null && n.MaSoThue.Contains(searchString))
                );
            }
            

            if (!string.IsNullOrEmpty(filterStatus))
            {
                bool isActive = filterStatus == "active";
                query = query.Where(n => n.IsActive == isActive);
            }
            
            var list = query.OrderByDescending(n => n.NgayTao).ToList();
            
            ViewBag.SearchString = searchString;
            ViewBag.FilterStatus = filterStatus;
            
            return View(list);
        }
        
        public IActionResult Create()
        {
            var model = new NhaTuyenDung
            {
                IsActive = true
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhaTuyenDung model)
        {
            if (model == null)
            {
                model = new NhaTuyenDung();
            }
            

            if (!string.IsNullOrEmpty(model.Email))
            {
                if (_context.NhaTuyenDung.Any(n => n.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhà tuyển dụng khác.");
                }
            }
            

            if (!string.IsNullOrEmpty(model.MaSoThue))
            {
                if (_context.NhaTuyenDung.Any(n => n.MaSoThue == model.MaSoThue))
                {
                    ModelState.AddModelError("MaSoThue", "Mã số thuế này đã được sử dụng bởi nhà tuyển dụng khác.");
                }
            }
            
            if (ModelState.IsValid)
            {
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = null;
                _context.NhaTuyenDung.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Thêm nhà tuyển dụng thành công.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.NhaTuyenDung.Find(id);
            if (model == null)
                return NotFound();
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(NhaTuyenDung model)
        {

            if (!string.IsNullOrEmpty(model.Email))
            {
                if (_context.NhaTuyenDung.Any(n => n.Email == model.Email && n.Id != model.Id))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng bởi nhà tuyển dụng khác.");
                }
            }
            

            if (!string.IsNullOrEmpty(model.MaSoThue))
            {
                if (_context.NhaTuyenDung.Any(n => n.MaSoThue == model.MaSoThue && n.Id != model.Id))
                {
                    ModelState.AddModelError("MaSoThue", "Mã số thuế này đã được sử dụng bởi nhà tuyển dụng khác.");
                }
            }
            
            if (ModelState.IsValid)
            {
                var existingModel = _context.NhaTuyenDung.Find(model.Id);
                if (existingModel == null)
                {
                    return NotFound();
                }
                

                existingModel.TenCongTy = model.TenCongTy;
                existingModel.MaSoThue = model.MaSoThue;
                existingModel.DiaChi = model.DiaChi;
                existingModel.SoDienThoai = model.SoDienThoai;
                existingModel.Email = model.Email;
                existingModel.Website = model.Website;
                existingModel.QuyMo = model.QuyMo;
                existingModel.LinhVuc = model.LinhVuc;
                existingModel.MoTa = model.MoTa;
                existingModel.IsActive = model.IsActive;
                existingModel.NgayCapNhat = DateTime.Now;
                
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật nhà tuyển dụng thành công.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.NhaTuyenDung
                .Include(n => n.TinTuyenDungs)
                .FirstOrDefault(n => n.Id == id);
            if (model == null)
                return NotFound();
            
            return View(model);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var model = _context.NhaTuyenDung
                .Include(n => n.TinTuyenDungs)
                .FirstOrDefault(n => n.Id == id);
            if (model == null)
                return NotFound();
            
            ViewBag.HasTinTuyenDung = model.TinTuyenDungs != null && model.TinTuyenDungs.Any();
            
            return View(model);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.NhaTuyenDung
                .Include(n => n.TinTuyenDungs)
                .FirstOrDefault(n => n.Id == id);
            
            if (model == null)
                return NotFound();
            

            if (model.TinTuyenDungs != null && model.TinTuyenDungs.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà tuyển dụng này. Có " + model.TinTuyenDungs.Count() + " tin tuyển dụng đang liên quan.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            try
            {
                _context.NhaTuyenDung.Remove(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa nhà tuyển dụng thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhà tuyển dụng này. Có thể đang có dữ liệu liên quan.";
                return RedirectToAction("Delete", new { id = id });
            }
            
            return RedirectToAction("Index");
        }
    }
}

