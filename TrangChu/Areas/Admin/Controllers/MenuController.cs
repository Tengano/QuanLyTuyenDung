using TrangChu.Models;
using TrangChu.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public IActionResult Index()
        {
            var mnList = _context.Menu.OrderBy(m => m.ID).ToList();
            return View(mnList);
        }
        
            public IActionResult Create()
        {
            var mnList = _context.Menu.Select(m => new SelectListItem
            {
                Text = m.Name ?? "",
                Value = m.ID.ToString()
            }).ToList();
            
            mnList.Insert(0, new SelectListItem { Text = "--- select ---", Value = "0" });
            ViewBag.mnList = mnList;
            
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Menu mn)
        {
            if (ModelState.IsValid)
            {
                _context.Menu.Add(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            // Reload ViewBag for dropdown if validation fails
            var mnList = _context.Menu.Select(m => new SelectListItem
            {
                Text = m.Name ?? "",
                Value = m.ID.ToString()
            }).ToList();
            
            mnList.Insert(0, new SelectListItem { Text = "--- select ---", Value = "0" });
            ViewBag.mnList = mnList;
            
            return View(mn);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            
            var mn = _context.Menu.Find(id);
            if (mn == null)
                return NotFound();
            
            // Create dropdown list for parent menu
            var mnList = _context.Menu.Select(m => new SelectListItem
            {
                Text = m.Name ?? "",
                Value = m.ID.ToString()
            }).ToList();
            
            mnList.Insert(0, new SelectListItem { Text = "--- select ---", Value = "0" });
            ViewBag.mnList = mnList;
            
            return View(mn);
        }
        
        [HttpPost]
        public IActionResult Edit(Menu mn)
        {
            if (ModelState.IsValid)
            {
                _context.Menu.Update(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            // Reload ViewBag for dropdown if validation fails
            var mnList = _context.Menu.Select(m => new SelectListItem
            {
                Text = m.Name ?? "",
                Value = m.ID.ToString()
            }).ToList();
            
            mnList.Insert(0, new SelectListItem { Text = "--- select ---", Value = "0" });
            ViewBag.mnList = mnList;
            
            return View(mn);
        }
        
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return NotFound();
            var mn = _context.Menu.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var delMenu = _context.Menu.Find(id);
            if (delMenu == null)
                return NotFound();
            _context.Menu.Remove(delMenu);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}