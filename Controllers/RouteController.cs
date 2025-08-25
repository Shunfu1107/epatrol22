using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering; // Add this for SelectListItem

namespace AdminPortalV8.Controllers
{
    public class RouteController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        public RouteController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);
                userId = 0;

                var routes = await _context.Routes
                                    .Include(cp => cp.RouteCheckPoints)
                                        .ThenInclude(rcp => rcp.CheckPoint)
                                    .Include(cp => cp.PatrolType)
                                    .Include(r => r.RouteSchedules) // ✅ Fix: Use RouteSchedules
                                        .ThenInclude(rs => rs.Schedule)
                                    .ToListAsync();

                ViewBag.Routes = routes;

                var checkpoints = await _context.CheckPoints.ToListAsync();
                ViewBag.Checkpoints = checkpoints;

                var schedules = await _context.Schedules.ToListAsync();
                ViewBag.Schedules = schedules;

                // Add duration options to ViewBag (in seconds)
                ViewBag.DurationOptions = new List<SelectListItem>
                {
                    new SelectListItem { Text = "15 seconds", Value = "15" },
                    new SelectListItem { Text = "30 seconds", Value = "30" },
                    new SelectListItem { Text = "1 minute", Value = "60" },
                    new SelectListItem { Text = "2 minutes", Value = "120" },
                    new SelectListItem { Text = "5 minutes", Value = "300" }
                };
                
                ViewBag.IntervalOptions = new List<SelectListItem>
                {                    
                    new SelectListItem { Text = "30 minutes", Value = "1800" },
                    new SelectListItem { Text = "1 hours", Value = "3600" },
                    new SelectListItem { Text = "2 hours", Value = "7200" },
                    new SelectListItem { Text = "3 hours", Value = "10800" }
                };

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewRoute(string RouteName, int PatrolTypeId, IFormFile patrolMap, List<int> txtCheckpoint, List<int> txtSchedule, int Duration, int SleepTime)
        {
            try
            {
                byte[]? imageData = null;

                if (patrolMap != null && (patrolMap.ContentType == "image/jpeg" || patrolMap.ContentType == "image/png"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await patrolMap.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                var newRoute = new Models.Epatrol.Route
                {
                    RouteName = RouteName,
                    PatrolTypeId = PatrolTypeId,
                    Image = imageData,
                    Duration = Duration, // Save duration in seconds
                    SleepTime = SleepTime
                };

                _context.Routes.Add(newRoute);
                await _context.SaveChangesAsync();

                // Add checkpoints
                foreach (var checkpointId in txtCheckpoint)
                {
                    var routeCheckPoint = new RouteCheckPoint
                    {
                        RouteId = newRoute.RouteId,
                        CheckPointId = checkpointId
                    };
                    _context.RouteCheckPoints.Add(routeCheckPoint);
                }

                await _context.SaveChangesAsync();

                // Add schedules
                foreach (var scheduleId in txtSchedule)
                {
                    var routeSchedule = new RouteSchedule
                    {
                        RouteId = newRoute.RouteId,
                        ScheduleId = scheduleId
                    };
                    _context.RouteSchedules.Add(routeSchedule);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRoute(int RouteId)
        {
            try
            {
                var route = await _context.Routes
                    .Include(cp => cp.RouteCheckPoints)
                    .Include(cp => cp.RouteSchedules)
                    .FirstOrDefaultAsync(cp => cp.RouteId == RouteId);

                if (route == null)
                {
                    return NotFound();
                }

                _context.RouteCheckPoints.RemoveRange(route.RouteCheckPoints);
                _context.RouteSchedules.RemoveRange(route.RouteSchedules);
                route.Schedules.Clear();

                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to delete the route. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetCheckpointsByType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Json(Array.Empty<object>());

            type = type.Trim();

            IQueryable<CheckPoint> q = _context.CheckPoints;

            if (type.Equals("Manual", StringComparison.OrdinalIgnoreCase))
            {
                // Checkpoint is Manual if it has at least one Manual checklist
                q = q.Where(cp => cp.CheckLists.Any(cl => cl.Type != null && cl.Type == "Manual"));
            }
            else if (type.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                // Auto = has Auto but no Manual (Manual wins when mixed)
                q = q.Where(cp =>
                    cp.CheckLists.Any(cl => cl.Type != null && cl.Type == "Auto") &&
                   !cp.CheckLists.Any(cl => cl.Type != null && cl.Type == "Manual"));
            }
            else
            {
                return Json(Array.Empty<object>());
            }

            var data = await q.OrderBy(cp => cp.CheckPointName)
                              .Select(cp => new { id = cp.CheckPointId, text = cp.CheckPointName })
                              .ToListAsync();

            return Json(data);
        }


        public IActionResult GetCheckPoints(int routeId)
        {
            try
            {
                var route = _context.Routes
                                    .Include(r => r.RouteCheckPoints)
                                        .ThenInclude(rp => rp.CheckPoint)
                                    .FirstOrDefault(r => r.RouteId == routeId);

                if (route == null)
                {
                    return NotFound();
                }

                var response = new
                {
                    RelatedCheckPoints = route.RouteCheckPoints.Select(rp => new
                    {
                        CheckPointId = rp.CheckPoint.CheckPointId,
                        CheckPointName = rp.CheckPoint.CheckPointName
                    }).ToList(),
                    AllCheckPoints = _context.CheckPoints.Select(cp => new
                    {
                        CheckPointId = cp.CheckPointId,
                        CheckPointName = cp.CheckPointName
                    }).ToList()
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public IActionResult GetSchedules(int routeId)
        {
            try
            {
                var routeSchedules = _context.RouteSchedules
                    .Where(rs => rs.RouteId == routeId)
                    .Include(rs => rs.Schedule)
                    .ToList();

                if (routeSchedules == null)
                {
                    return NotFound();
                }

                var response = new
                {
                    RelatedSchedules = routeSchedules.Select(rs => new
                    {
                        ScheduleId = rs.Schedule.ScheduleId,
                        ScheduleName = rs.Schedule.ScheduleName
                    }).ToList(),
                    AllSchedules = _context.Schedules.Select(s => new
                    {
                        ScheduleId = s.ScheduleId,
                        ScheduleName = s.ScheduleName
                    }).ToList()
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRoute(int RouteId, string RouteName, int PatrolTypeId, IFormFile PatrolMap, List<int> txtEditCheckpoint, List<int> txtEditSchedule, int Duration, int SleepTime)
        {
            try
            {
                var route = await _context.Routes
                    .Include(r => r.RouteCheckPoints)
                        .ThenInclude(rp => rp.CheckPoint)
                    .Include(r => r.RouteSchedules)
                    .FirstOrDefaultAsync(r => r.RouteId == RouteId);

                if (route == null)
                {
                    return NotFound();
                }

                route.RouteName = RouteName;
                route.PatrolTypeId = PatrolTypeId;
                route.Duration = Duration; // Update duration in seconds
                route.SleepTime = SleepTime;

                if (PatrolMap != null && (PatrolMap.ContentType == "image/jpeg" || PatrolMap.ContentType == "image/png"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await PatrolMap.CopyToAsync(memoryStream);
                        route.Image = memoryStream.ToArray();
                    }
                }

                _context.RouteCheckPoints.RemoveRange(route.RouteCheckPoints);

                var checkpoints = await _context.CheckPoints.Where(c => txtEditCheckpoint.Contains(c.CheckPointId)).ToListAsync();
                foreach (var checkpoint in checkpoints)
                {
                    var routeCheckPoint = new RouteCheckPoint
                    {
                        RouteId = route.RouteId,
                        CheckPointId = checkpoint.CheckPointId
                    };
                    route.RouteCheckPoints.Add(routeCheckPoint);
                }

                var existingSchedules = _context.RouteSchedules.Where(rs => rs.RouteId == RouteId);
                _context.RouteSchedules.RemoveRange(existingSchedules);

                foreach (var scheduleId in txtEditSchedule)
                {
                    var routeSchedule = new RouteSchedule
                    {
                        RouteId = RouteId,
                        ScheduleId = scheduleId
                    };
                    _context.RouteSchedules.Add(routeSchedule);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
