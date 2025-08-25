using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AdminPortalV8.Controllers
{
    public class PatrollingController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;

        public PatrollingController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
        }

        [HttpGet]
        public IActionResult Index(int routeId)
        {
            var route = _context.Routes
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                        .ThenInclude(cp => cp.Location)
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                        .ThenInclude(cp => cp.CheckLists)
                .Include(r => r.RouteCheckPoints)
                    .ThenInclude(rcp => rcp.CheckPoint)
                        .ThenInclude(cp => cp.Cameras)
                .FirstOrDefault(r => r.RouteId == routeId);

            if (route == null)
            {
                return NotFound();
            }

            var checkpoints = new List<CheckpointViewModel>();

            foreach (var rcp in route.RouteCheckPoints)
            {
                double x = 0, y = 0;

                if (!string.IsNullOrEmpty(rcp.Coordinate))
                {
                    var parts = rcp.Coordinate.Split(',');
                    if (parts.Length == 3)
                    {
                        double.TryParse(parts[1], out x);
                        double.TryParse(parts[2], out y);
                    }
                }

                checkpoints.Add(new CheckpointViewModel
                {
                    CheckpointId = rcp.CheckPoint.CheckPointId,
                    CheckpointName = rcp.CheckPoint.CheckPointName,
                    X = x,
                    Y = y,
                    FloorId = rcp.CheckPoint.Location?.LocationId ?? 0,
                    FloorName = rcp.CheckPoint.Location?.Name ?? "Unknown",
                    Level = rcp.CheckPoint.Location?.Level ?? "Unknown",
                    Checklists = rcp.CheckPoint.CheckLists.Select(cl => new ChecklistItemViewModel
                    {
                        ChecklistId = cl.CheckListId,
                        ChecklistName = cl.CheckListName
                    }).ToList()
                });
            }

            var floors = route.RouteCheckPoints
                .Where(rcp => rcp.CheckPoint?.Location != null)
                .Select(rcp => new {
                    FloorId = rcp.CheckPoint.Location.LocationId,
                    FloorName = rcp.CheckPoint.Location.Name,
                    Level = rcp.CheckPoint.Location.Level,
                    MapBase64 = rcp.CheckPoint.Location.Image != null
                        ? Convert.ToBase64String(rcp.CheckPoint.Location.Image)
                        : ""
                })
                .GroupBy(f => f.FloorId)
                .Select(g => g.First())
                .OrderBy(f => f.FloorName)
                .ToList();

            ViewBag.Floors = floors;

            ViewBag.CheckpointsWithCoordinates = checkpoints.Select((cp, index) => new {
                Index = index,
                cp.CheckpointId,
                cp.CheckpointName,
                X = cp.X,  // make sure these are decimal / float from DB
                Y = cp.Y,
                cp.FloorId,
                cp.FloorName,
                cp.Level
            }).ToList();

            var model = new PatrollingViewModel
            {
                RouteId = route.RouteId,
                RouteName = route.RouteName,
                MapBase64 = route.RouteCheckPoints
                    .Select(rcp => rcp.CheckPoint?.Location?.Image)
                    .Where(img => img != null)
                    .Select(img => Convert.ToBase64String(img))
                    .FirstOrDefault() ?? "",
                Checkpoints = checkpoints
            };

            foreach (var rcp in route.RouteCheckPoints)
            {
                if (rcp.CheckPoint == null)
                    Console.WriteLine("CheckPoint is null");
                else if (rcp.CheckPoint.Location == null)
                    Console.WriteLine("Location is null");
                else if (rcp.CheckPoint.Location.Image == null)
                    Console.WriteLine("Image is null");
            }


            return View(model);
        }

        [HttpGet]
        public IActionResult GetCheckpoints(int routeId)
        {
            var route = _context.Routes
                .Where(r => r.RouteId == routeId)
                .Select(r => new
                {
                    Checkpoints = r.RouteCheckPoints.Select(cp => new
                    {
                        CheckpointId = cp.CheckPointId,
                        CheckpointName = cp.CheckPoint.CheckPointName,
                        Checklists = cp.CheckPoint.CheckLists.Select(cl => new
                        {
                            ChecklistId = cl.CheckListId,
                            ChecklistName = cl.CheckListName
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (route == null)
            {
                return NotFound();
            }

            return Json(route);
        }

        [HttpGet]
        public IActionResult GetGuards()
        {
            var guards = _context.Guards
                .Select(g => new
                {
                    GuardId = g.GuardId, // Ensure this is correct
                    Name = g.Name
                })
                .ToList();
            return Json(guards);
        }

        [HttpPost]
        public IActionResult SavePatrol([FromBody] PatrolData patrolData)
        {
            if (patrolData == null || !patrolData.Checklists.Any())
            {
                return BadRequest("No data to save.");
            }

            try
            {
                foreach (var checklist in patrolData.Checklists)
                {
                    var patrol = new Patrol
                    {
                        RouteId = patrolData.RouteId,
                        CheckPointName = checklist.CheckPointName,
                        CheckListName = checklist.CheckListName,
                        Status = checklist.Status,
                        Note = checklist.Note,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Time = TimeOnly.FromDateTime(DateTime.Now),
                        GuardName = patrolData.GuardName,
                        //GuardName = "Hardedcoded",
                        VideoLink = null,
                        Link = null
                    };
                    _context.Patrols.Add(patrol);
                }

                _ffmpegProcessService.StopProcess();
                _context.SaveChanges();

                return Ok("Patrol data saved successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting if needed
                // _logger.LogError(ex, "Error occurred while saving patrol data.");

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult PatrolDone()
        {
            return RedirectToAction("Index", "Patrol");
        }

        [HttpPost]
        public IActionResult StopStreaming()
        {
            _ffmpegProcessService.StopProcess();
            return Ok();
        }
    }
}
