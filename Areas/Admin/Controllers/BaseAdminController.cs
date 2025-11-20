using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TrangChu.Models;

namespace TrangChu.Areas.Admin.Controllers
{
    public class BaseAdminController : Controller
    {
        private static readonly HashSet<string> EmployerAllowedControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Home",
            "TinTuyenDung",
            "HoSoUngTuyen",
            "QuaTrinhPhongVan",
            "KetQuaPhongVan"
        };
        
        protected bool IsAdmin =>
            string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase);
        
        protected bool IsEmployer =>
            string.Equals(HttpContext.Session.GetString("UserRole"), "Employer", StringComparison.OrdinalIgnoreCase);
        
        protected int? EmployerCompanyId =>
            HttpContext.Session.GetInt32("EmployerCompanyId");
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("UserRole");
            
            var logger = context.HttpContext.RequestServices.GetService<ILogger<BaseAdminController>>();
            var employerCompanyId = context.HttpContext.Session.GetInt32("EmployerCompanyId");
            logger?.LogInformation("BaseAdminController: UserId={UserId}, UserRole={UserRole}, EmployerCompanyId={EmployerCompanyId}, Path={Path}", 
                userId, userRole, employerCompanyId, context.HttpContext.Request.Path);
            
            if (userId == null || string.IsNullOrEmpty(userRole) || (userRole != "Admin" && userRole != "Employer"))
            {
                logger?.LogWarning("BaseAdminController: Access denied. UserId={UserId}, UserRole={UserRole}", userId, userRole);
                
                if (!context.HttpContext.Response.HasStarted)
                {
                    context.Result = new RedirectResult("/Account/Login");
                }
                return;
            }
            
            if (string.Equals(userRole, "Employer", StringComparison.OrdinalIgnoreCase))
            {
                EnsureEmployerHasCompany(context, userId.Value, logger);
                
                var controllerName = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
                if (!EmployerAllowedControllers.Contains(controllerName))
                {
                    logger?.LogWarning("BaseAdminController: Employer tried to access restricted controller {ControllerName}", controllerName);
                    context.Result = new RedirectToActionResult("Index", "Home", new { area = "Admin" });
                    return;
                }
            }
            
            logger?.LogInformation("BaseAdminController: Access granted for UserId={UserId}, UserRole={UserRole}", userId, userRole);
        }
        
        protected int? GetEmployerCompanyId()
        {
            return HttpContext.Session.GetInt32("EmployerCompanyId");
        }
        
        private void EnsureEmployerHasCompany(ActionExecutingContext context, int userId, ILogger? logger)
        {
            var employerCompanyId = context.HttpContext.Session.GetInt32("EmployerCompanyId");
            if (employerCompanyId.HasValue)
            {
                return;
            }
            
            var dataContext = context.HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;
            if (dataContext == null)
            {
                logger?.LogError("BaseAdminController: Unable to resolve DataContext for employer company check.");
                return;
            }
            
            var account = dataContext.TaiKhoan
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == userId);
            
            if (account?.NhaTuyenDungId == null)
            {
                logger?.LogWarning("BaseAdminController: Employer account {UserId} has no linked company.", userId);
                context.HttpContext.Session.Remove("EmployerCompanyId");
                return;
            }
            
            context.HttpContext.Session.SetInt32("EmployerCompanyId", account.NhaTuyenDungId.Value);
            logger?.LogInformation("BaseAdminController: Cached EmployerCompanyId={CompanyId} for UserId={UserId}", account.NhaTuyenDungId, userId);
        }
    }
}
