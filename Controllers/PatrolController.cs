using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using AdminPortalV8.Hubs;
using AdminPortalV8.ViewModels;
using EPatrol.Services;

namespace AdminPortalV8.Controllers
{
    public class PatrolController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAutoPtrolApiCalling _autoPatrolRequest;

        public PatrolController(
            UserObj usrObj,
            IGeneral general,
            EPatrol_DevContext context,
            IFfmpegProcessService ffmpegProcessService,
            IHubContext<NotificationHub> hubContext,
            IAutoPtrolApiCalling autoPatrolRequest)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _hubContext = hubContext;
            _autoPatrolRequest = autoPatrolRequest;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);

                var routes = await _context.Routes
                    .Include(cp => cp.RouteCheckPoints)
                        .ThenInclude(rcp => rcp.CheckPoint)
                    .Include(cp => cp.PatrolType)
                    .Include(r => r.RouteSchedules)
                        .ThenInclude(rs => rs.Schedule)
                    .ToListAsync();
                ViewBag.Routes = routes;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartAutoPatrol(int routeId)
        {
            var route = _context.Routes
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                    .ThenInclude(cp => cp.Cameras)
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                    .ThenInclude(cp => cp.CheckLists)
                    .ThenInclude(cl => cl.ModelNavigation)
                .Include(r => r.RouteSchedules)
                    .ThenInclude(rs => rs.Schedule)
                .FirstOrDefault(r => r.RouteId == routeId);

            if (route == null)
            {
                return Json(new { success = false, message = "Route not found." });
            }

            if (route.RouteCheckPoints == null || !route.RouteCheckPoints.Any())
            {
                return Json(new { success = false, message = "No checkpoints found for this route." });
            }

            var cameraIp = route.RouteCheckPoints
                .SelectMany(rcp => rcp.CheckPoint.Cameras)
                .FirstOrDefault()?.Url;

            if (string.IsNullOrEmpty(cameraIp))
            {
                return Json(new { success = false, message = "No camera found for this route." });
            }

            var models = route.RouteCheckPoints
                .SelectMany(rcp => rcp.CheckPoint.CheckLists)
                .Select(cl => new ModelInfo { name = cl.ModelNavigation?.Name, url = cl.ModelNavigation?.Url })
                .Where(m => m.name != null && m.url != null)
                .ToList();

            if (!models.Any())
            {
                return Json(new { success = false, message = "No AI models found for this route." });
            }

            var duration = route.Duration.ToString();
            var schedules = route.RouteSchedules?.Select(rs => rs.Schedule).ToList();
            if (schedules == null || !schedules.Any())
            {
                return Json(new { success = false, message = "No schedules found for this route." });
            }

            string today = DateTime.Today.DayOfWeek.ToString();
            var schedule = schedules.FirstOrDefault(s => s.Day == today);
            if (schedule == null)
            {
                return Json(new { success = false, message = $"No schedule found for today ({today})." });
            }

            if (!schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
            {
                return Json(new { success = false, message = "Schedule start or end time is not set." });
            }

            var localTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTz);
            var todayDate = currentTime.Date;
            var scheduleStartTime = todayDate.Add(schedule.StartTime.Value.ToTimeSpan());
            var scheduleEndTime = todayDate.Add(schedule.EndTime.Value.ToTimeSpan());

            if (currentTime > scheduleEndTime)
            {
                return Json(new { success = false, message = "Cannot start patrol: The scheduled end time has already passed." });
            }

            double delaySeconds = currentTime < scheduleStartTime ? (scheduleStartTime - currentTime).TotalSeconds : 0;

            var autoDetectionData = new AutoDetectionData
            {
                Message = "HELLO",
                RouteId = routeId.ToString(),
                DropBox_Token = "your_dropbox_token_here",
                Duration = duration,
                RouteName = route.RouteName ?? "Unknown Route",
                CameraIp = cameraIp,
                Models = models,
                ScheduleStartTime = scheduleStartTime.ToString("o"),
                ScheduleEndTime = scheduleEndTime.ToString("o"),
                DelaySeconds = delaySeconds
            };

            try
            {
                var response = await _autoPatrolRequest.SendAutoPatrlRequestAsync(
                    autoDetectionData.CameraIp,
                    int.Parse(autoDetectionData.Duration),
                    autoDetectionData.DropBox_Token,
                    autoDetectionData.Models,
                    autoDetectionData.ScheduleStartTime,
                    autoDetectionData.ScheduleEndTime,
                    autoDetectionData.DelaySeconds
                );

                route.AutoPatrol = true;
                await _context.SaveChangesAsync();

                _ffmpegProcessService.StartProcess(route.RouteCheckPoints.First().CheckPointId, cameraIp);
                return Json(new { success = true, message = $"Auto patrol scheduled successfully. Will start in {delaySeconds} seconds." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to schedule auto patrol: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StopAutoPatrol(int routeId, string cameraIp)
        {
            var route = _context.Routes.FirstOrDefault(r => r.RouteId == routeId);
            if (route == null)
            {
                return Json(new { success = false, message = "Route not found." });
            }

            var autoDetectionData = new AutoDetectionData
            {
                Message = "HELLO",
                RouteId = routeId.ToString(),
                CameraIp = cameraIp
            };

            var jsonData = JsonConvert.SerializeObject(autoDetectionData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await new HttpClient().PostAsync("http://localhost:8001/stop_patrolling", content);

            if (response.IsSuccessStatusCode)
            {
                route.AutoPatrol = false;
                await _context.SaveChangesAsync();

                _ffmpegProcessService.StopProcess();
                return Json(new { success = true, message = "Auto patrol stopped successfully." });
            }

            return Json(new { success = false, message = "Failed to stop auto patrol." });
        }

        [HttpGet]
        public IActionResult StartPatrolling(int routeId)
        {
            var route = _context.Routes
                .Include(r => r.PatrolType)
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                    .ThenInclude(cp => cp.Cameras)
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                    .ThenInclude(cp => cp.CheckLists)
                .FirstOrDefault(r => r.RouteId == routeId);

            if (route == null)
            {
                TempData["ErrorMessage"] = "Error: Route not found.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (route.RouteCheckPoints == null || !route.RouteCheckPoints.Any())
            {
                TempData["ErrorMessage"] = "This route has no checkpoints. Please add at least one checkpoint.";
                return RedirectToAction("Index", "Route");
            }

            var cameraUrls = route.RouteCheckPoints
                .Select(cp => new { cp.CheckPointId, CameraUrl = cp.CheckPoint.Cameras.FirstOrDefault()?.Url })
                .Where(cam => !string.IsNullOrEmpty(cam.CameraUrl))
                .ToList();

            if (cameraUrls.Count == 0)
            {
                TempData["ErrorMessage"] = "No active cameras available for this route. Please add at least one camera.";
                return RedirectToAction("Index", "Cameras");
            }

            foreach (var cam in cameraUrls)
            {
                _ffmpegProcessService.StartProcess(cam.CheckPointId, cam.CameraUrl);
            }

            System.Threading.Thread.Sleep(6000);

            TempData["InfoMessage"] = "FFmpeg processes started for all route cameras.";
            if (route.PatrolType.Name.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "AutoPatrolling", new { routeId });
            }
            else
            {
                return RedirectToAction("Index", "Patrolling", new { routeId });
            }
        }
    }
}