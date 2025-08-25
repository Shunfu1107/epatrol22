using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using AdminPortalV8.Helpers;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Data;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using AdminPortalV8.Models.Epatrol;
using Microsoft.AspNetCore.SignalR;
using AdminPortalV8.Hubs;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IHubContext<NotificationHub> _hubContext; // Add SignalR hub context

        public HomeController(
            IDashboardService dashboard,
            UserObj usrObj,
            IGeneral general,
            EPatrol_DevContext context,
            IHubContext<NotificationHub> hubContext) // Inject hub context
        {
            _dashboard = dashboard;
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _hubContext = hubContext; // Initialize hub context
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                // Check if userId is null and handle accordingly
                if (_usrObj?.user?.Id == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);

                return View();
            }
            catch (FormatException ex)
            {
                // Handle invalid user ID format
                Console.WriteLine($"FormatException: {ex.Message}");
                return BadRequest("Invalid user ID format.");
            }
            catch (Exception ex)
            {
                // Catch any other exception and log it
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        [ContentPermission(
            Key = AppModuleKeys.Dashboard.ViewPrimaryKey,
            Title = AppModuleKeys.Dashboard.ViewTitle,
            Desc = AppModuleKeys.Dashboard.ViewDescription,
            StaticAuthorization = AppModuleKeys.Dashboard.ViewStaticAuthorized
            )]
        public IActionResult Details()
        {
            return View();
        }

        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);

                var notifications = await _context.Patrols
                    .Where(p => p.Note == "Anomalies Detected!" && p.Date == todayDate)
                    .Include(p => p.Route)
                    .OrderByDescending(p => p.Time)
                    .Select(p => new
                    {
                        RouteName = p.Route.RouteName,
                        CheckPointName = p.CheckPointName,
                        Note = p.Note,
                        Time = string.Format("{0:hh\\:mm}", p.Time),
                    })
                    .ToListAsync();

                // Send notifications to all connected clients via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notifications);

                return Json(new { success = true, data = notifications });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}