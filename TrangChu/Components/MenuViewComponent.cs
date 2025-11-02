using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrangChu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TrangChu.Components

{
    [ViewComponent(Name = "MenuView")]
    public class MenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public MenuViewComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Load all menus from database
                var allMenus = await _context.tblMenu.ToListAsync();
                
                // Return empty list if no menus found
                if (allMenus == null || !allMenus.Any())
                {
                    return View(new List<tblMenu>());
                }
                
                // Build hierarchy: assign children to their parents
                var menuDict = allMenus.ToDictionary(m => m.id);
                foreach (var menu in allMenus)
                {
                    if (menu.parent_id.HasValue && menuDict.ContainsKey(menu.parent_id.Value))
                    {
                        var parent = menuDict[menu.parent_id.Value];
                        if (parent.Children == null)
                        {
                            parent.Children = new List<tblMenu>();
                        }
                        parent.Children.Add(menu);
                    }
                }
                
                // Get top-level menus (parent_id is null) and order them
                var listMenu = allMenus
                    .Where(m => m.parent_id == null)
                    .OrderBy(m => m.order_index ?? int.MaxValue)
                    .ThenBy(m => m.id)
                    .ToList();
                
                // Sort children recursively
                void SortChildren(tblMenu menuItem)
                {
                    if (menuItem.Children != null && menuItem.Children.Any())
                    {
                        menuItem.Children = menuItem.Children
                            .OrderBy(c => c.order_index ?? int.MaxValue)
                            .ThenBy(c => c.id)
                            .ToList();
                        foreach (var child in menuItem.Children)
                        {
                            SortChildren(child);
                        }
                    }
                }
                
                foreach (var menu in listMenu)
                {
                    SortChildren(menu);
                }
                
                return View(listMenu);
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"MenuViewComponent Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return empty list on error
                return View(new List<tblMenu>());
            }
        }
    }
}