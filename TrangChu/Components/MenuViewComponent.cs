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
                var allMenus = await _context.Menu.ToListAsync();
                
                // Return empty list if no menus found
                if (allMenus == null || !allMenus.Any())
                {
                    return View(new List<Menu>());
                }
                
                // Build hierarchy: assign children to their parents
                var menuDict = allMenus.ToDictionary(m => m.ID);
                foreach (var menu in allMenus)
                {
                    if (menu.ParentID.HasValue && menuDict.ContainsKey(menu.ParentID.Value))
                    {
                        var parent = menuDict[menu.ParentID.Value];
                        if (parent.Children == null)
                        {
                            parent.Children = new List<Menu>();
                        }
                        parent.Children.Add(menu);
                    }
                }
                
                // Get top-level menus (parent_id is null) and order them
                var listMenu = allMenus
                    .Where(m => m.ParentID == null)
                    .OrderBy(m => m.OrderIndex ?? int.MaxValue)
                    .ThenBy(m => m.ID)
                    .ToList();
                
                // Sort children recursively
                void SortChildren(Menu menuItem)
                {
                    if (menuItem.Children != null && menuItem.Children.Any())
                    {
                        menuItem.Children = menuItem.Children
                            .OrderBy(c => c.OrderIndex ?? int.MaxValue)
                            .ThenBy(c => c.ID)
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
                return View(new List<Menu>());
            }
        }
    }
}