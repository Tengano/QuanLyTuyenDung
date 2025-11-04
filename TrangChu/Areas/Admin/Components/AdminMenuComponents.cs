using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangChu.Models;
using TrangChu.Areas.Admin.Models;

namespace TrangChu.Areas.Admin.Components
{
    [ViewComponent(Name = "AdminMenu")]
    public class AdminMenuComponent : ViewComponent
    {
        private readonly DataContext _context;

        public AdminMenuComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mnList = await ( from mn in _context.AdminMenu
                            where (mn.IsActive == true)
                            select mn).ToListAsync();
            return View("Default", mnList);
        }
    }
}

