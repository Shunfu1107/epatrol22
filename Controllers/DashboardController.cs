using System.Text;
using AdminPortalV8.Hubs;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using EPatrol.Services;
using AdminPortalV8.ViewModels;
using AdminPortalV8.Models;
using System.Linq;
using SixLabors.ImageSharp;

namespace AdminPortalV8.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly IAutoPtrolApiCalling _autoPatrolRequest;

        public DashboardController(
            IDashboardService dashboard,
            UserObj usrObj,
            IGeneral general,
            EPatrol_DevContext context,
            IHubContext<NotificationHub> hubContext,
            IFfmpegProcessService ffmpegProcessService,
            IAutoPtrolApiCalling autoPtrolApiCalling)
        {
            _dashboard = dashboard;
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _hubContext = hubContext;
            _ffmpegProcessService = ffmpegProcessService;
            _autoPatrolRequest = autoPtrolApiCalling;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
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
                Console.WriteLine($"FormatException: {ex.Message}");
                return BadRequest("Invalid user ID format.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public IActionResult GetRouteSummary()
        {
            try
            {
                string currentDay = DateTime.Now.DayOfWeek.ToString();
                DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);

                // Get total routes count
                var totalRoutes = _context.Routes.Count();

                // Get today's routes count (routes scheduled for today)
                var todayRoutes = _context.RouteSchedules
                    .Include(rs => rs.Schedule)
                    .Where(rs => rs.Schedule != null && rs.Schedule.Day == currentDay)
                    .Select(rs => rs.RouteId)
                    .Distinct()
                    .Count();

                // Get completed routes count (routes that have been patrolled today)
                var completedRoutes = _context.Patrols
                    .Where(p => p.Date == todayDate)
                    .Select(p => p.RouteId)
                    .Distinct()
                    .Count();

                return Json(new
                {
                    totalRoutes,
                    todayRoutes,
                    completedRoutes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRouteSummary: {ex.Message}");
                return Json(new
                {
                    totalRoutes = 0,
                    todayRoutes = 0,
                    completedRoutes = 0
                });
            }
        }

        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            try
            {
                DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);
                string today = DateTime.Today.DayOfWeek.ToString();

                var totalCameras = _context.Cameras.Count();
                var workingCameras = _context.Cameras.Count(c => c.IsActive == true);
                var faultyCameras = totalCameras - workingCameras;

                var latestStatusByCamera = _context.CameraStatusNews
                    .GroupBy(c => c.CameraId)
                    .Select(g => g.OrderByDescending(x => x.StatusDate).FirstOrDefault())
                    .ToList();

                var blurryCameras = latestStatusByCamera.Count(c => c.Status == "Blurry");
                var darkCameras = latestStatusByCamera.Count(c => c.Status == "Dark");

                var totalModels = _context.Aimodels.Count();
                var modelsInUse = _context.Aimodels.Include(m => m.CheckLists).ToList().Count(m => m.CheckLists.Any());

                var routeSchedules = _context.RouteSchedules
                    .Include(rs => rs.Schedule)
                    .Include(rs => rs.Route)
                        .ThenInclude(r => r.PatrolType)
                    .ToList();

                var manualToday = routeSchedules
                    .Count(rs => rs.Schedule?.Day == today && rs.Route?.PatrolType?.Name == "Manual");

                var autoToday = routeSchedules
                    .Count(rs => rs.Schedule?.Day == today && rs.Route?.PatrolType?.Name == "Auto");

                var patrols = _context.Patrols
                    .Include(p => p.Route)
                        .ThenInclude(r => r.PatrolType)
                    .Where(p => p.Date == todayDate)
                    .ToList();

                var manualCompleted = patrols
                    .Where(p => p.Route?.PatrolType?.Name == "Manual")
                    .Select(p => p.RouteId)
                    .Distinct()
                    .Count(); var autoCompleted = patrols.Count(p => p.Route?.PatrolType?.Name == "Auto");

                var camerasConfigured = _context.Cameras.Count();

                var continuousDetectionActive = _context.Cameras.Include(c => c.CameraCheckLists).ToList().Count(c => c.CameraCheckLists.Any());

                return Json(new
                {
                    totalCameras,
                    workingCameras,
                    faultyCameras,
                    blurryCameras,
                    darkCameras,
                    totalModels,
                    modelsInUse,
                    manualToday,
                    manualCompleted,
                    autoToday,
                    autoCompleted,
                    camerasConfigured,
                    continuousDetectionActive
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetDashboardStats error: {ex}");
                return Json(new { error = true, message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet]
        public IActionResult GetRouteData(int draw, int start, int length, string search)
        {
            int orderColumn = Convert.ToInt32(Request.Query["order[0][column]"]);
            string orderDir = Request.Query["order[0][dir]"];

            string today = DateTime.Today.DayOfWeek.ToString();
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);

            var query = _context.Routes
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                    .ThenInclude(cp => cp.Cameras)
                .Include(r => r.PatrolType)
                .Include(r => r.Patrols)
                .Include(r => r.Schedules)
                .Join(_context.RouteSchedules,
                      r => r.RouteId,
                      rs => rs.RouteId,
                      (r, rs) => new { Route = r, RouteSchedule = rs })
                .Where(r => r.RouteSchedule.Schedule != null && r.RouteSchedule.Schedule.Day == today)
                .Select(r => new
                {
                    r.Route.RouteId,
                    r.Route.RouteName,
                    CheckPointName = r.Route.RouteCheckPoints.Select(rcp => new
                    {
                        name = rcp.CheckPoint != null ? rcp.CheckPoint.CheckPointName : "Unknown",
                        cameraIp = rcp.CheckPoint != null && rcp.CheckPoint.Cameras.Any() ? rcp.CheckPoint.Cameras.First().Url : ""
                    }).ToList(),
                    ScheduleId = r.RouteSchedule.Schedule.ScheduleId,
                    r.Route.PatrolTypeId,
                    Image = r.Route.Image != null ? Convert.ToBase64String(r.Route.Image) : null,
                    PatrolTypeName = r.Route.PatrolType != null ? r.Route.PatrolType.Name : null,
                    Day = r.RouteSchedule.Schedule.Day,
                    ScheduleStartTime = r.RouteSchedule.Schedule.StartTime.HasValue ? r.RouteSchedule.Schedule.StartTime.Value.ToString("HH:mm") : null,
                    ScheduleEndTime = r.RouteSchedule.Schedule.EndTime.HasValue ? r.RouteSchedule.Schedule.EndTime.Value.ToString("HH:mm") : null,
                    Interval = r.Route.SleepTime.HasValue
                    ? (r.Route.SleepTime.Value / 60 >= 60
                        ? (r.Route.SleepTime.Value / 3600).ToString() + " hour(s)"
                        : (r.Route.SleepTime.Value / 60).ToString() + " min")
                    : "0 min",
                    PatrolStatus = r.Route.Patrols.Any(p => p.RouteId == r.Route.RouteId && p.Date.HasValue && p.Date.Value == todayDate)
                        ? "Patrolled"
                        : "Not Patrolled",
                    AutoPatrol = r.Route.AutoPatrol,
                    Duration = r.Route.Duration
                });

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.RouteName.Contains(search) ||
                                         r.RouteId.ToString().Contains(search));
            }

            query = orderColumn switch
            {
                1 => orderDir == "asc" ? query.OrderBy(r => r.RouteName) : query.OrderByDescending(r => r.RouteName),
                _ => query.OrderBy(r => r.RouteId)
            };

            var totalRecords = query.Count();
            var displayResult = query.Skip(start).Take(length).ToList();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult
            });
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

        [HttpGet]
        public async Task<IActionResult> GetStoredNotifications()
        {
            try
            {
                var list = await _context.Notifications
                    .Where(n => n.Clear == false)
                    .OrderByDescending(n => n.Timestamp)
                    .Select(n => new
                    {
                        device = n.Device,
                        type = n.Type,
                        location = n.Location,
                        timestamp = n.Timestamp,
                        note = n.Note
                    }).ToListAsync();

                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStoredNotifications: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }


        private async Task SaveNotification(string device, string type, string location, DateTime timestamp, string note)
        {
            var notification = new Notification
            {
                Device = device,
                Type = type,
                Location = location,
                Timestamp = timestamp,
                Note = note
            };

            if (!_context.Notifications.Any(n =>
                n.Device == device &&
                n.Type == type &&
                n.Timestamp == timestamp &&
                n.Location == location))
            {
                _context.Notifications.Add(new Notification
                {
                    Device = device,
                    Type = type,
                    Location = location,
                    Timestamp = timestamp,
                    Note = note
                });

                _context.SaveChanges();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcknowledgeNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.Acknowledge = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ClearNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null || !notification.Acknowledge) return BadRequest("Not acknowledged yet.");

            notification.Clear = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveNotifications()
        {
            var activeList = await _context.Notifications
                .Where(n => !n.Clear) // Still active
                .OrderByDescending(n => n.Timestamp)
                .Select(n => new
                {
                    n.Id,
                    n.Device,
                    n.Type,
                    n.Location,
                    n.Timestamp,
                    n.Note,
                    n.Acknowledge,
                    n.Clear
                }).ToListAsync();

            return Json(new { success = true, data = activeList });
        }

        [HttpGet]
        public async Task<IActionResult> GetPastNotifications()
        {
            var pastList = await _context.Notifications
                .Where(n => n.Clear) // Cleared already
                .OrderByDescending(n => n.Timestamp)
                .Select(n => new
                {
                    n.Id,
                    n.Device,
                    n.Type,
                    n.Location,
                    n.Timestamp,
                    n.Note
                }).ToListAsync();

            return Json(new { success = true, data = pastList });
        }


        public async Task<IActionResult> GetNotification()
        {
            try
            {
                DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);
                DateTime todayStart = DateTime.Today;

                var notifications = new List<object>();

                var healthList = await _context.CameraHealthResults
                    .Where(c => c.Timestamp >= todayStart && c.Status != "Working")
                    .OrderByDescending(c => c.Timestamp)
                    .ToListAsync();

                foreach (var health in healthList)
                {
                    var device = health.CameraName;
                    var type = "Camera Health";
                    var location = health.BlockName + " " + health.Level;
                    var timestamp = health.Timestamp;
                    var note = health.Result;

                    notifications.Add(new
                    {
                        device,
                        type,
                        location,
                        timestamp,
                        note
                    });

                    await SaveNotification(device, type, location, timestamp, note);
                }

                var statusList = await _context.CameraStatusNews
                    .Include(c => c.Camera).ThenInclude(cam => cam.Location)
                    .Where(c => c.StatusDate >= todayStart && c.Status == "Not Working")
                    .OrderByDescending(c => c.StatusDate)
                    .ToListAsync();

                foreach (var status in statusList)
                {
                    var device = status.Camera?.Name;
                    var type = "Camera Status";
                    var location = status.Camera?.Location?.Name + " " + status.Camera?.Location?.Level;
                    var timestamp = status.StatusDate;
                    var note = status.Note;

                    notifications.Add(new
                    {
                        device,
                        type,
                        location,
                        timestamp,
                        note
                    });

                    await SaveNotification(device, type, location, timestamp, note);
                }

                var fireList = await _context.FireDetecteds
                    .Include(f => f.Camera).ThenInclude(cam => cam.Location)
                    .Where(f => f.CreatedAt >= todayStart)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                foreach (var fire in fireList)
                {
                    var device = fire.Camera?.Name;
                    var type = "Continuous Surveillance";
                    var location = fire.Camera?.Location?.Name + " " + fire.Camera?.Location?.Level;
                    var timestamp = fire.CreatedAt.GetValueOrDefault(DateTime.Now);
                    var note = "Anomally detected";

                    notifications.Add(new
                    {
                        device,
                        type,
                        location,
                        timestamp,
                        note
                    });

                    await SaveNotification(device, type, location, timestamp, note);
                }

                var patrolList = await _context.Patrols
                    .Include(p => p.Route)
                    .Where(p => p.Note == "Anomalies Detected!" && p.Date == todayDate)
                    .OrderByDescending(p => p.Time)
                    .ToListAsync();

                foreach (var patrol in patrolList)
                {
                    var timestamp = DateTime.Today.Add((patrol.Time ?? TimeOnly.MinValue).ToTimeSpan());
                    notifications.Add(new
                    {
                        device = patrol.Route?.RouteName,
                        type = "Patrol",
                        location = patrol.CheckPointName,
                        timestamp,
                        note = patrol.Note
                    });

                    await SaveNotification(patrol.Route?.RouteName, "Patrol", patrol.CheckPointName, timestamp, patrol.Note);
                }

                return Json(new { success = true, data = notifications });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNotifications: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ToggleAutoPatrol(int routeId, bool enable)
        {
            var route = await _context.Routes.FindAsync(routeId);
            if (route == null)
            {
                return Json(new { success = false, message = "Route not found." });
            }

            route.AutoPatrol = enable;
            await _context.SaveChangesAsync();

            string status = enable ? "enabled" : "disabled";
            return Json(new { success = true, message = $"AutoPatrol {status} successfully." });
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

        [HttpGet]
        public async Task<IActionResult> GetCameraHealthNotifications()
        {
            DateTime todayStart = DateTime.Today;

            var list = await _context.CameraHealthResults
                .Where(c => c.Timestamp >= todayStart && c.Status != "Working")
                .OrderByDescending(c => c.Timestamp)
                .Select(c => new
                {
                    device = c.CameraName,
                    type = "Camera Health",
                    location = c.BlockName + " " + c.Level,
                    timestamp = c.Timestamp
                }).ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetCameraStatusNotifications()
        {
            DateTime todayStart = DateTime.Today;

            var list = await _context.CameraStatusNews
                .Include(c => c.Camera)
                    .ThenInclude(cam => cam.Location)
                .Where(c => c.StatusDate >= todayStart && c.Status != "Working")
                .OrderByDescending(c => c.StatusDate)
                .Select(c => new
                {
                    device = c.Camera.Name,
                    type = "Camera Status",
                    location = c.Camera.Location != null ? c.Camera.Location.Name : "-",
                    timestamp = c.StatusDate,
                    status = c.Status
                })
                .ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetFireNotifications()
        {
            DateTime todayStart = DateTime.Today;

            var list = await _context.FireDetecteds
                .Include(f => f.Camera)
                    .ThenInclude(cam => cam.Location)
                .Where(f => f.CreatedAt >= todayStart)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new
                {
                    device = f.Camera.Name,
                    type = "Continuous Surveillance",
                    location = f.Camera.Location != null ? f.Camera.Location.Name : "-",
                    timestamp = f.CreatedAt
                }).ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatrolNotifications()
        {
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Today);
            DateTime todayStart = DateTime.Today;

            var list = await _context.Patrols
                .Include(p => p.Route)
                .Where(p => p.Date >= todayDate && p.Status == "Warning")
                .OrderByDescending(p => p.Time)
                .Select(p => new
                {
                    device = p.Route.RouteName,
                    type = "Patrol",
                    location = p.CheckPointName,
                    timestamp = (p.Date.HasValue && p.Time.HasValue)
                        ? p.Date.Value.ToDateTime(p.Time.Value)
                        : DateTime.MinValue
                }).ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public IActionResult GetSavedCameraSelections()
        {
            if (_usrObj.user?.Id == null)
                return Unauthorized();

            int userId = Convert.ToInt32(_usrObj.user.Id);

            var saved = _context.UserCameraSelections
                .Where(s => s.UserId == userId)
                .Select(s => s.CheckPointId)
                .ToList();

            return Json(new { success = true, checkpoints = saved });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCameraSelection([FromBody] CameraSelectionModel model)
        {
            if (_usrObj.user?.Id == null)
                return Unauthorized();

            int userId = Convert.ToInt32(_usrObj.user.Id);

            if (model?.SelectedCheckPoints == null || model.SelectedCheckPoints.Count < 1 || model.SelectedCheckPoints.Count > 6)
            {
                return BadRequest("Invalid number of selected cameras.");
            }

            // Remove all existing selections for this user
            var existing = await _context.UserCameraSelections
                .Where(x => x.UserId == userId)
                .ToListAsync();
            _context.UserCameraSelections.RemoveRange(existing);

            // Add the new selections
            foreach (var checkpointId in model.SelectedCheckPoints)
            {
                _context.UserCameraSelections.Add(new UserCameraSelection
                {
                    UserId = userId,
                    CheckPointId = checkpointId,
                    UpdatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }


        [HttpGet]
        public IActionResult GetCameraStreams()
        {
            try
            {
                var userId = _usrObj?.user?.Id;
                int intUserId = Convert.ToInt32(userId);

                var streams = _context.Cameras
                    .Include(c => c.CheckPoints)
                    .Where(c => c.IsActive)
                    .Select(c => new
                    {
                        name = c.Name,
                        url = c.Url,
                        checkPointId = c.CheckPoints.Select(cp => cp.CheckPointId).FirstOrDefault()
                    })
                    .ToList();

                var savedCheckpoints = _context.UserCameraSelections
                    .Where(s => s.UserId == intUserId)
                    .Select(s => s.CheckPointId)
                    .ToList();

                return Json(new { success = true, data = streams });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCameraSelections([FromBody] List<int> checkpointIds)
        {
            if (_usrObj.user?.Id == null)
                return Unauthorized();

            int userId = Convert.ToInt32(_usrObj.user.Id);

            var existing = _context.UserCameraSelections.Where(s => s.UserId == userId);
            _context.UserCameraSelections.RemoveRange(existing);

            var newSelections = checkpointIds.Select(id => new UserCameraSelection
            {
                UserId = userId,
                CheckPointId = id,
                UpdatedAt = DateTime.Now
            }).ToList();

            _context.UserCameraSelections.AddRange(newSelections);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> StartCamera(string cameraUrl, int checkPointId)
        {
            if (string.IsNullOrEmpty(cameraUrl))
            {
                TempData["ErrorMessage"] = "No camera URL provided.";
                return BadRequest("Camera URL is required.");
            }

            try
            {
                _ffmpegProcessService.StartProcess(checkPointId, cameraUrl);
                await Task.Delay(5000); // Let stream stabilize (shorten delay)
                TempData["InfoMessage"] = "FFmpeg process started for the specified camera.";
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error starting camera stream.", error = ex.Message });

            }

        }

        [HttpGet]
        public IActionResult StopCamera(int checkPointId)
        {
            try
            {
                _ffmpegProcessService.StopProcess(checkPointId);
                return Ok(new { message = $"Camera stream for checkpoint {checkPointId} stopped." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error stopping camera stream.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult StopAllCameras()
        {
            try
            {
                _ffmpegProcessService.StopProcess();
                return Ok(new { message = "All camera streams stopped." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error stopping all camera streams.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult StopStreaming()
        {
            _ffmpegProcessService.StopProcess();
            return Ok();
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            try
            {
                var locations = _context.Locations
                    .Select(l => new
                    {
                        Id = l.LocationId,
                        Building = l.Name,
                        Floor = l.Level
                    })
                    .ToList();

                return Json(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAvailableBuildingLocations()
        {
            try
            {
                var locations = _context.Locations
                .Select(l => new {
                    LocationId = l.LocationId,
                    Building = l.Name,
                    Floor = l.Level
                }).ToList();

                var usedLocationIds = _context.Buildings
                    .Where(b => b.LocationId != null)
                    .Select(b => b.LocationId)
                    .ToList();

                var usedLocationNames = _context.Buildings
                    .Select(b => b.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name.Trim())
                    .Distinct()
                    .ToList();

                return Json(new
                {
                    success = true,
                    locations,
                    usedLocationIds,
                    usedLocationNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllBuildings()
        {
            var buildings = _context.Buildings
                .ToList() // bring to memory first
                .Select(b => new {
                    Id = b.Id,
                    Name = b.Name,
                    ImageName = b.ImageName,
                    Floors = _context.Locations
                        .Where(l => l.Name == b.Name)
                        .AsEnumerable() // switch to LINQ-to-Objects
                        .Select(l => l.Level?.Trim())
                        .Distinct()
                        .OrderBy(level => GetFloorSortKey(level))
                        .ToList()
                }).ToList();

            return Ok(buildings);
        }



        [HttpGet]
        public IActionResult GetAllBuildingslist()
        {
            var buildings = _context.Buildings
                .Select(b => new {
                    Id = b.Id,
                    Name = b.Name,
                    ImageName = b.ImageName,
                    NumberOfFloors = _context.Locations.Count(l => l.Name == b.Name)
                }).ToList();

            return Ok(buildings);
        }

        private int GetFloorSortKey(string level)
        {
            if (string.IsNullOrEmpty(level)) return int.MaxValue;

            level = level.Trim().ToUpper();

            if (level.StartsWith("B") && int.TryParse(level.Substring(1), out int basementNumber))
                return -100 + basementNumber * -1;  // e.g., B3 => -103, B1 => -101

            if (level == "LG") return -50;
            if (level == "G") return 0;

            if (int.TryParse(level, out int floorNumber))
                return floorNumber;

            return int.MaxValue - 1; // Unknown labels go to the bottom
        }


        [HttpGet]
        public IActionResult GetFloorImage(string buildingName, string floor)
        {
            try
            {
                var location = _context.Locations
                    .FirstOrDefault(l => l.Name == buildingName && l.Level == floor);

                if (location?.Image != null)
                {
                    return File(location.Image, "image/jpeg");
                }

                return NotFound(new { status = "no_floor_plan" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult SaveBuilding([FromBody] BuildingModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.BuildingName) || model.FloorCount <= 0 || string.IsNullOrWhiteSpace(model.Image) || model.LocationId <= 0)
                return BadRequest("Missing fields");

            var building = new Building
            {
                Name = model.BuildingName,
                NumberOfFloors = model.FloorCount,
                ImageName = model.Image,
                LocationId = model.LocationId
            };

            _context.Buildings.Add(building);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetFloorCameraSetpoints(string buildingName, string floor)
        {
            var locations = _context.Locations
                .Where(l => l.Name == buildingName && l.Level == floor)
                .Select(l => l.LocationId)
                .ToList();

            var cameras = _context.Cameras
                .Where(c => c.LocationId.HasValue && locations.Contains(c.LocationId.Value) && !string.IsNullOrEmpty(c.Coordinate))
                .Select(c => new
                {
                    c.CameraId,
                    c.Name,
                    c.Url,
                    c.Coordinate,
                    c.LocationId
                })
                .ToList();

            return Json(cameras);
        }

        [HttpGet]
        public IActionResult GetAllCameraStreams()
        {
            try
            {
                var streams = _context.Cameras
                 .Include(c => c.CheckPoints)
                 .SelectMany(c => c.CheckPoints.Select(cp => new {
                     cameraId = c.CameraId,
                     name = c.Name,
                     url = c.Url,
                     checkPointId = cp.CheckPointId
                 }))
                 .ToList();

                return Json(new { success = true, data = streams });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllLocations()
        {
            var locations = _context.Locations
                .Select(l => new {
                    locationId = l.LocationId,
                    buildingName = l.Name,
                    level = l.Level
                })
                .ToList();

            return Json(locations);
        }

        [HttpPost]
        public IActionResult UploadBuildingImage([FromForm] int buildingId, [FromForm] string imageName)
        {
            var building = _context.Buildings.FirstOrDefault(b => b.Id == buildingId);
            if (building == null)
            {
                return NotFound("Building not found.");
            }

            building.ImageName = imageName;
            _context.SaveChanges();

            return Ok(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteBuilding(int id)
        {
            var building = _context.Buildings.FirstOrDefault(b => b.Id == id);
            if (building == null)
                return NotFound();

            _context.Buildings.Remove(building);
            _context.SaveChanges();

            return Ok();
        }


        public class CameraSelectionModel
        {
            public List<int> SelectedCheckPoints { get; set; }
        }

        public class BuildingModel
        {
            public string BuildingName { get; set; }
            public int FloorCount { get; set; }
            public string Image { get; set; }
            public int LocationId { get; set; }
        }
    }
}