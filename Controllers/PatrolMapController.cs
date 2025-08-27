using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPatrol.Services;

namespace AdminPortalV8.Controllers
{
    public class PatrolMapController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly IImageQualityAnalyzer _imageQualityAnalyzer;
        
        public PatrolMapController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, IImageQualityAnalyzer imageQualityAnalyzer)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _imageQualityAnalyzer = imageQualityAnalyzer;
        }
        public async Task<IActionResult> Index(int? routeId)
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);
                userId = 0;

                var routes = await _context.Routes
                    .Select(route => new
                    {
                        RouteId = route.RouteId,
                        RouteName = route.RouteName,
                        Checkpoints = _context.RouteCheckPoints
                            .Where(rp => rp.RouteId == route.RouteId)
                            .Select(rp => new
                            {
                                CheckPointId = rp.CheckPointId,
                                CheckPointName = rp.CheckPoint.CheckPointName,
                                CameraIds = rp.CameraId,
                                Coordinate = rp.Coordinate,
                                LocationId = rp.CheckPoint.Location != null ? rp.CheckPoint.Location.LocationId : (int?)null,
                                ImageData = rp.CheckPoint.Location != null && rp.CheckPoint.Location.Image != null
                                    ? Convert.ToBase64String(rp.CheckPoint.Location.Image)
                                    : null
                            })
                            .ToList()
                    })
                    .ToListAsync();

                ViewBag.Routes = routes;
                ViewBag.SelectedRouteId = routeId;

                var checkpointsWithCameras = new List<object>();

                foreach (var route in routes)
                {
                    foreach (var rp in route.Checkpoints)
                    {
                        var cameraIds = rp.CameraIds?.Split(',') ?? Array.Empty<string>();
                        var coordinateArray = rp.Coordinate?.Split('%') ?? Array.Empty<string>();

                        for (int i = 0; i < cameraIds.Length; i++)
                        {
                            if (int.TryParse(cameraIds[i], out int cameraId) && i < coordinateArray.Length)
                            {
                                var coordinates = coordinateArray[i]?.Split(',') ?? Array.Empty<string>();

                                if (coordinates.Length == 3 && int.TryParse(coordinates[0], out int coordCameraId) && coordCameraId == cameraId)
                                {
                                    var xCoordinate = coordinates[1];
                                    var yCoordinate = coordinates[2];

                                    // Fetch camera details
                                    var cameraDetail = await _context.Cameras
                                        .Where(c => c.CameraId == cameraId)
                                        .Select(c => new { c.CameraId, c.Url, c.Name })
                                        .FirstOrDefaultAsync();

                                    if (cameraDetail != null)
                                    {
                                        checkpointsWithCameras.Add(new
                                        {
                                            route.RouteId,
                                            route.RouteName,
                                            rp.CheckPointId,
                                            rp.CheckPointName,
                                            rp.LocationId,
                                            CameraId = cameraDetail.CameraId,
                                            CameraName = cameraDetail.Name,
                                            CameraUrl = cameraDetail.Url,
                                            X = xCoordinate,
                                            Y = yCoordinate,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                ViewBag.RouteCheckpoints = checkpointsWithCameras;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the data.");
            }
        }

        public JsonResult GetCamerasByRoute(int routeId)
        {
            var cameras = _context.RouteCheckPoints
                .Where(rcp => rcp.RouteId == routeId)
                .SelectMany(rcp => rcp.CheckPoint.Cameras, (rcp, cam) => new
                {
                    CameraId = cam.CameraId,
                    CameraName = cam.Name,
                    CheckPointId = rcp.CheckPoint.CheckPointId,
                    CheckPointName = rcp.CheckPoint.CheckPointName,
                    LocationId = rcp.CheckPoint.LocationId
                })
                .ToList();

            return Json(cameras);
        }

        public class CombinedUpdateModel
        {
            public int RouteId { get; set; }
            public int CameraId { get; set; }
            public string CheckPointName { get; set; }
            public int CoordX { get; set; }
            public int CoordY { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCameraRouteCheckpoint([FromBody] CombinedUpdateModel model)
        {
            if (model == null || model.RouteId <= 0 || model.CameraId <= 0 || string.IsNullOrEmpty(model.CheckPointName))
            {
                return Json(new { success = false, message = "Invalid input" });
            }

            try
            {
                // Save CameraId
                var checkPointName = model.CheckPointName.Trim();
                var checkPoint = await _context.CheckPoints
                    .FirstOrDefaultAsync(cp => cp.CheckPointName.ToLower() == checkPointName.ToLower());

                if (checkPoint == null)
                {
                    return Json(new { success = false, message = "Checkpoint not found." });
                }

                var existingRouteCheckPoint = await _context.RouteCheckPoints
                    .FirstOrDefaultAsync(rc => rc.RouteId == model.RouteId && rc.CheckPointId == checkPoint.CheckPointId);

                string newCoordinateSet = $"{model.CameraId},{model.CoordX},{model.CoordY}";

                if (existingRouteCheckPoint != null)
                {
                    if (string.IsNullOrEmpty(existingRouteCheckPoint.CameraId))
                    {
                        existingRouteCheckPoint.CameraId = model.CameraId.ToString();
                        existingRouteCheckPoint.Coordinate = newCoordinateSet;
                    }
                    else
                    {
                        if (existingRouteCheckPoint.CameraId.Split(',').Contains(model.CameraId.ToString()))
                        {
                            return Json(new { success = true, message = "CameraId already exists, no update made.", routeId = model.RouteId });
                        }

                        existingRouteCheckPoint.CameraId += $",{model.CameraId}";

                        if (!string.IsNullOrEmpty(existingRouteCheckPoint.Coordinate))
                        {
                            existingRouteCheckPoint.Coordinate += $"%{newCoordinateSet}";
                        }
                        else
                        {
                            existingRouteCheckPoint.Coordinate = newCoordinateSet;
                        }
                    }
                }
                else
                {
                    var routeCheckPoint = new RouteCheckPoint
                    {
                        RouteId = model.RouteId,
                        CheckPointId = checkPoint.CheckPointId,
                        CameraId = model.CameraId.ToString(),
                        Coordinate = newCoordinateSet
                    };

                    _context.RouteCheckPoints.Add(routeCheckPoint);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "CameraId updated successfully.", routeId = model.RouteId });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex.Message}");
                return StatusCode(500, "Database update error: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public class DeleteCameraRequest
        {
            public int RouteId { get; set; }
            public int CheckPointId { get; set; }
            public int CameraId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCamera([FromBody] DeleteCameraRequest request)
        {
            var routeCheckpoint = await _context.RouteCheckPoints
                .FirstOrDefaultAsync(rc => rc.RouteId == request.RouteId && rc.CheckPointId == request.CheckPointId);

            if (routeCheckpoint != null)
            {
                var camera = await _context.Cameras.FindAsync(request.CameraId);
                var checkpoint = await _context.CheckPoints.FindAsync(request.CheckPointId);

                if (camera == null || checkpoint == null)
                {
                    return Json(new { success = false, message = "Camera or CheckPoint not found." });
                }

                var cameraIds = routeCheckpoint.CameraId.Split(',')
                    .Where(id => id != request.CameraId.ToString()).ToArray();

                routeCheckpoint.CameraId = string.Join(",", cameraIds);

                var coordinates = routeCheckpoint.Coordinate.Split('%')
                    .Where(coordSet => !coordSet.StartsWith(request.CameraId + ","))
                    .ToArray();

                routeCheckpoint.Coordinate = string.Join("%", coordinates);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    routeId = request.RouteId
                });
            }
            else
            {
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult StartCamera(string cameraUrl, int checkPointId)
        {
            if (string.IsNullOrEmpty(cameraUrl))
            {
                TempData["ErrorMessage"] = "No camera URL provided.";
                return BadRequest("Camera URL is required.");
            }

            _ffmpegProcessService.StartProcess(checkPointId, cameraUrl);

            System.Threading.Thread.Sleep(6000); // Wait for 6 seconds to ensure stream stability

            TempData["InfoMessage"] = "FFmpeg process started for the specified camera.";
            return Ok();
        }

        [HttpGet]
        public IActionResult StopCamera()
        {
            try
            {
                _ffmpegProcessService.StopProcess();
                TempData["InfoMessage"] = "FFmpeg process stopped for the specified camera.";
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error stopping FFmpeg process.";
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult StopStreaming()
        {
            _ffmpegProcessService.StopProcess();
            return Ok();
        }
    }
}
