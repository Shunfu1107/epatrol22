using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EPatrol.Services;

namespace AdminPortalV8.Controllers
{
    public class FloorMapController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly IImageQualityAnalyzer _imageQualityAnalyzer;

        public FloorMapController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, IImageQualityAnalyzer imageQualityAnalyzer)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _imageQualityAnalyzer = imageQualityAnalyzer;
        }
        public async Task<IActionResult> Index(int? buildingId)
        {
            if (buildingId.HasValue)
            {
                var locations = (from loc in _context.Locations
                                 join b in _context.Buildings on loc.Name equals b.Name
                                 where b.Id == buildingId.Value
                                 select new LocationViewModel
                                 {
                                     LocationId = loc.LocationId,
                                     BuildingName = b.Name,
                                     FloorName = loc.Level
                                 }).OrderBy(x => x.FloorName).ToList();

                ViewBag.Locations = locations;
            }
            else
            {
                ViewBag.Locations = new List<LocationViewModel>();
            }

            ViewBag.Filter = true;
            try
            {
                //var userId = Convert.ToInt32(_usrObj.user.Id);
                //var lists = await _general.GetPermissionDefault(userId);
                //userId = 0;

                var locations = await _context.Locations
                    .Select(l => new
                    {
                        l.LocationId,
                        BuildingName = l.Name,
                        FloorName = l.Level,
                        ImageData = l.Image != null ? Convert.ToBase64String(l.Image) : null
                    }).ToListAsync();


                var allCameras = await _context.Cameras
                    .Include(c => c.Location)
                    .Select(c => new
                    {
                        c.CameraId,
                        c.Name,
                        c.Url,
                        c.Coordinate,
                        LocationId = c.LocationId,
                        Building = c.Location.Name,
                        Floor = c.Location.Level
                    }).ToListAsync();

                ViewBag.Locations = locations;
                ViewBag.AllCameras = allCameras;

                return View();
            }
            catch (Exception ex)
            {
                return Content("Index Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCamerasByLocation(int locationId)
        {
            try
            {
                var cameras = await _context.Cameras
                    .Where(c => c.LocationId == locationId)
                    .Select(c => new
                    {
                        c.CameraId,
                        c.Name,
                        c.Url,
                        c.Coordinate
                    })
                    .ToListAsync();

                return Json(cameras);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving cameras", error = ex.Message });
            }
        }

        public class SaveCameraCoordinateModel
        {
            public int CameraId { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        [HttpPost]
        public IActionResult SaveCameraCoordinate([FromBody] SaveCameraCoordinateModel request)
        {
            var camera = _context.Cameras.FirstOrDefault(c => c.CameraId == request.CameraId);
            if (camera == null)
                return Json(new { success = false, message = "Camera not found" });

            camera.Coordinate = $"{camera.CameraId},{request.X},{request.Y}";
            _context.SaveChanges();

            return Json(new { success = true });
        }

        public class DeleteCameraCoordinateModel
        {
            public int CameraId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCameraCoordinate([FromBody] DeleteCameraCoordinateModel model)
        {
            var camera = await _context.Cameras.FindAsync(model.CameraId);
            if (camera == null)
                return Json(new { success = false, message = "Camera not found." });

            camera.Coordinate = null;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
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

        [HttpGet]
        public JsonResult GetLocationsByBuilding(int buildingId)
        {
            var locations = (from loc in _context.Locations
                             join b in _context.Buildings on loc.Name equals b.Name
                             where b.Id == buildingId
                             select new
                             {
                                 LocationId = loc.LocationId,
                                 BuildingName = b.Name,
                                 FloorName = loc.Level
                             }).OrderBy(x => x.FloorName).ToList();

            return Json(locations);
        }

        public class LocationViewModel
        {
            public int LocationId { get; set; }
            public string BuildingName { get; set; }
            public string FloorName { get; set; }
        }
    }
}
