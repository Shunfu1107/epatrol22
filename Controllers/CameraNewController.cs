using System.Runtime.InteropServices;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Text.Json;
namespace AdminPortalV8.Controllers
{
    public class CameraNewController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly ITelegramService _telegramService;
        private readonly ISMSService _smsService;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _schedulerCameraCheck;

        public CameraNewController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, ITelegramService telegramService, ISMSService sMSService, ISchedulerFactory schedulerFactory)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _telegramService = telegramService;
            _smsService = sMSService;
            _schedulerFactory = schedulerFactory;
            _schedulerCameraCheck = _schedulerFactory.GetScheduler().Result;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;

            if (_usrObj?.user == null)
            {
                return StatusCode(500, "User information is unavailable.");
            }

            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);

                var lists = await _general.GetPermissionDefault(userId);

                userId = 0; //pls comment if use restaurant filter

                // Fetching Patrol data
                var locationCameras = await _context.LocationCameras.Include(lc => lc.Location).ToListAsync();
                var locations = await _context.Locations.ToListAsync();

                if (locationCameras == null || !locationCameras.Any() || locations == null || !locations.Any())
                {
                    Console.WriteLine("No data found for LocationCameras or Locations.");
                    return StatusCode(500, "Required data is missing.");
                }

                ViewBag.LocationCameras = locationCameras;
                ViewBag.Locations = locations;


                Console.WriteLine($"record: {JsonSerializer.Serialize(locationCameras)}");


                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> GetLevels(string name)
        {
            var locationLevels = await _context.Locations
                                        .Where(l => l.Name == name)
                                        .Select(l => l.Level)
                                        .Distinct()
                                        .ToListAsync();

            return Json(locationLevels);
        }

        //public async Task<IActionResult> GetLocationId(string name, string level)
        //{
        //    // Query to get the LocationId based on name and level
        //    var location = await _context.Locations
        //                                  .Where(l => l.Name == name && l.Level == level)
        //                                  .FirstOrDefaultAsync();

        //    if (location != null)
        //    {
        //        return Json(location.LocationId);  // Return the LocationId
        //    }
        //    return Json(null);  // Return null if no matching location found
        //}

        public async Task<IActionResult> GetCamerasByLocationId(string name, string level)
        {
            var location = await _context.Locations
                                         .Where(l => l.Name == name && l.Level == level)
                                         .FirstOrDefaultAsync();

            if (location != null)
            {
                var locationCameras = await _context.LocationCameras
                                                    .Where(lc => lc.LocationId == location.LocationId)
                                                    .ToListAsync();

                return Json(locationCameras); // Return filtered camera data
            }

            return Json(new List<AdminPortalV8.Models.Epatrol.LocationCamera>()); // Return empty list if no match
        }



        [HttpPost]
        public IActionResult AddCamera(string selectedName, string selectedLevel, string Name, string RtspUrl)
        {
            //var locations = _context.Locations.ToList();
            //Console.WriteLine($"record selectedName: {selectedName}, selectedLevel: {selectedLevel}, Name: {Name}, RtspUrl: {RtspUrl}");

            // Find the location based on the selected block and level
            var location = _context.Locations
                .FirstOrDefault(l => l.Name == selectedName && l.Level == selectedLevel);
            //};

            if (location == null)
            {
                return Json(new { success = false, message = "Location not found for the selected block and level." });
            }

            // Get the LocationId from the found location
            int locationId = location.LocationId;

            // Check for duplicate RTSP URL
            var duplicateCameraRTSPUrl = _context.LocationCameras
                .FirstOrDefault(c => c.RtspUrl == RtspUrl);

            if (duplicateCameraRTSPUrl != null)
            {
                return Json(new { success = false, message = "The RTSP URL already exists for another camera." });
            }

            // Create a new camera object (assuming you have a Camera model)
            var locationCamera = new LocationCamera
            {
                LocationId = locationId,
                Name = Name,
                RtspUrl = RtspUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                // Save the new camera record to the database
                _context.LocationCameras.Add(locationCamera);
                _context.SaveChanges();

                // Return success response
                return Json(new { success = true, message = "Camera added successfully." });
            }
            catch (Exception ex)
            {
                // Handle error and return failure response
                return Json(new { success = false, message = "Error adding camera: " + ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int LocationCameraId, string Name, string RtspUrl)
        {
            var locationcamera = await _context.LocationCameras.FindAsync(LocationCameraId);
            if (locationcamera == null)
            {
                TempData["ErrorMessage"] = "Camera not found.";
                return RedirectToAction("Index");
            }

            locationcamera.Name = Name;
            locationcamera.RtspUrl = RtspUrl;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int locationCameraId)
        {
            try
            {
                var locationCamera = await _context.LocationCameras.FindAsync(locationCameraId);
                if (locationCamera == null)
                {
                    TempData["ErrorMessage"] = "Camera not found.";
                    return RedirectToAction("Index");
                }

                _context.LocationCameras.Remove(locationCamera);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete the camera. Please try again.";
            }

            return RedirectToAction("Index");
        }

        //new camera check
        //[HttpPost]
        //[Route("CameraNew/CheckAllCameras")]
        //public async Task<IActionResult> CheckAllCameras()
        //{
        //    // Fetch all cameras from the database
        //    var locationCameras = await _context.LocationCameras.ToListAsync();
        //    if (locationCameras == null || !locationCameras.Any())
        //    {
        //        return Json(new { success = false, message = "No cameras found to check." });
        //    }

        //    // Use a background task or Quartz scheduler to handle the process
        //    foreach (var locationCamera in locationCameras)
        //    {
        //        var jobKey = new JobKey($"CameraCheck_{locationCamera.LocationCameraId}", "CheckAllGroup");
        //        var triggerKey = new TriggerKey($"Trigger_CameraCheck_{locationCamera.LocationCameraId}", "CheckAllGroup");

        //        if (!await _schedulerCameraCheck.CheckExists(jobKey))
        //        {
        //            // Create a job for checking the camera health
        //            IJobDetail job = JobBuilder.Create<AutoCameraCheckNew>()
        //                .WithIdentity(jobKey)
        //                .UsingJobData("RTSPUrl", locationCamera.RtspUrl)
        //                .UsingJobData("locationCameraName", locationCamera.Name)
        //                .UsingJobData("locationCameraId", locationCamera.LocationCameraId.ToString())
        //                .Build();

        //            // Create a simple trigger to fire the job immediately
        //            ITrigger trigger = TriggerBuilder.Create()
        //                .WithIdentity(triggerKey)
        //                .StartNow()
        //                .Build();

        //            await _schedulerCameraCheck.ScheduleJob(job, trigger);
        //        }
        //    }

        //    return Json(new { success = true, message = "Camera check initiated for all cameras." });
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetCameraCheckResults(int page = 1, int pageSize = 20, string cameraName = "", string status = "", DateTime? startDate = null, DateTime? endDate = null)
        //{
        //    try
        //    {
        //        var query = _context.CameraStatusesNew
        //            .Include(cs => cs.LocationCamera)
        //            .AsQueryable();

        //        // Apply filters
        //        if (!string.IsNullOrEmpty(cameraName))
        //        {
        //            query = query.Where(cs => cs.LocationCamera.Name.Contains(cameraName));
        //        }
        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            query = query.Where(cs => cs.Status == status);
        //        }
        //        if (startDate.HasValue)
        //        {
        //            query = query.Where(cs => cs.StatusDate >= startDate.Value);
        //        }
        //        if (endDate.HasValue)
        //        {
        //            query = query.Where(cs => cs.StatusDate <= endDate.Value);
        //        }

        //        // Get total count for pagination
        //        var totalItems = await query.CountAsync();

        //        // Apply pagination
        //        var results = await query
        //            .OrderBy(cs => cs.StatusDate) // Order by timestamp
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(cs => new
        //            {
        //                CameraId = cs.LocationCameraId,
        //                CameraName = cs.LocationCamera.Name,
        //                Status = cs.Status,
        //                Message = cs.Note,
        //                Timestamp = cs.StatusDate.HasValue ? cs.StatusDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        //            })
        //            .ToListAsync();

        //        return Json(new
        //        {
        //            success = true,
        //            results,
        //            pagination = new
        //            {
        //                totalItems,
        //                currentPage = page,
        //                pageSize,
        //                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error retrieving results: " + ex.Message });
        //    }
        //}

    }
}

