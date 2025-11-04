using TrangChu.Models;
using TrangChu.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: Admin/Menu
        public IActionResult Index()
        {
            var mnList = _context.AdminMenu.OrderBy(m => m.ItemOrder).ToList();
            return View(mnList);
        }

        // GET: Admin/Menu/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminMenu = await _context.AdminMenu
                .FirstOrDefaultAsync(m => m.AdminMenuID == id);
            
            if (adminMenu == null)
            {
                return NotFound();
            }

            return View(adminMenu);
        }

        // GET: Admin/Menu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminMenu adminMenu)
        {
            try
            {
                // Remove AdminMenuID from ModelState since it's auto-generated
                ModelState.Remove("AdminMenuID");
                
                if (ModelState.IsValid)
                {
                    _context.Add(adminMenu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thêm menu thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }
            
            return View(adminMenu);
        }

        // GET: Admin/Menu/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminMenu = await _context.AdminMenu.FindAsync(id);
            if (adminMenu == null)
            {
                return NotFound();
            }
            
            return View(adminMenu);
        }

        // POST: Admin/Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AdminMenu adminMenu)
        {
            if (id != adminMenu.AdminMenuID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminMenu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật menu thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminMenuExists(adminMenu.AdminMenuID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }
            
            return View(adminMenu);
        }

        // GET: Admin/Menu/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminMenu = await _context.AdminMenu
                .FirstOrDefaultAsync(m => m.AdminMenuID == id);
            
            if (adminMenu == null)
            {
                return NotFound();
            }

            return View(adminMenu);
        }

        // POST: Admin/Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var adminMenu = await _context.AdminMenu.FindAsync(id);
            
            if (adminMenu != null)
            {
                _context.AdminMenu.Remove(adminMenu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa menu thành công!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool AdminMenuExists(long id)
        {
            return _context.AdminMenu.Any(e => e.AdminMenuID == id);
        }
    }
}
