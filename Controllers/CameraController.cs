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
using static AdminPortalV8.Helpers.AppModuleKeys;
using Quartz;
using System.Web.WebPages;

namespace AdminPortalV8.Controllers
{
    public class CameraController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly ITelegramService _telegramService;
        private readonly ISMSService _smsService;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _schedulerCameraCheck;

        public CameraController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, ITelegramService telegramService, ISMSService sMSService, ISchedulerFactory schedulerFactory)
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

            // Check if the user object is null
            if (_usrObj?.user == null)
            {
                return StatusCode(500, "User information is unavailable.");
            }

            try
            {
                // Ensure that the user ID is valid
                var userId = _usrObj?.user?.Id;
                if (userId == null)
                {
                    return StatusCode(500, "User ID is null.");
                }

                var userIdInt = Convert.ToInt32(userId);

                // Fetch the permission data (using _general.GetPermissionDefault)
                var lists = await _general.GetPermissionDefault(userIdInt);

                // If you plan to use restaurant filtering, ensure the code here is correct
                userId = "0"; // please comment out if using restaurant filter

                // Fetching Patrol data and locations from the database
                var cameras = await _context.Cameras.ToListAsync();
                var locations = await _context.Locations.ToListAsync();

                // Pass the cameras and locations to the view
                ViewBag.Cameras = cameras;
                ViewBag.Locations = locations;

                return View();
            }
            catch (Exception ex)
            {
                // Return detailed error information for debugging
                return StatusCode(500, $"An error occurred: {ex.Message} - StackTrace: {ex.StackTrace}");
            }
        }



        //[HttpGet]
        //public async Task<IActionResult> Index(string searchName, string searchLevel)
        //{
        //    ViewBag.Filter = true;
        //    ViewBag.SelectedLocation = searchName; // Preserve selected location in the dropdown
        //    ViewBag.SelectedLevel = searchLevel; // Preserve selected level in the dropdown

        //    var cameras = await _context.Cameras.ToListAsync();

        //    // Apply filtering based on user selection
        //    if (!string.IsNullOrEmpty(searchName))
        //    {
        //        cameras = cameras.Where(c => c.Location.Name == searchName).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(searchLevel))
        //    {
        //        cameras = cameras.Where(c => c.Location.Level.ToString() == searchLevel).ToList();
        //    }

        //    ViewBag.Cameras = cameras;
        //    ViewBag.Locations = await _context.Locations.ToListAsync();

        //    return View();
        //}


        public async Task<IActionResult> GetLevels(string name)
        {
            var locationLevels = await _context.Locations
                                        .Where(l => l.Name == name)
                                        .Select(l => l.Level)
                                        .Distinct()
                                        .ToListAsync();

            return Json(locationLevels);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Name, string Url)
        {
            var existingCamera = await _context.Cameras.FirstOrDefaultAsync(c => c.Url == Url);
            if (existingCamera != null)
            {
                TempData["ErrorMessage"] = "A camera with this URL already exists.";
                return RedirectToAction("Index");
            }

            var camera = new Camera
            {
                Name = Name,
                Url = Url
            };

            _context.Cameras.Add(camera);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int CameraId)
        {
            try
            {
                var camera = await _context.Cameras.FindAsync(CameraId);
                if (camera == null)
                {
                    TempData["ErrorMessage"] = "Camera not found.";
                    return RedirectToAction("Index");
                }

                _context.Cameras.Remove(camera);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete the camera. Please try again.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetCamerasByLocationId(string name, string level)
        {
            IQueryable<Camera> query = _context.Cameras;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(level))
            {
                var location = await _context.Locations
                                             .Where(l => l.Name == name && l.Level == level)
                                             .FirstOrDefaultAsync();

                if (location != null)
                {
                    query = query.Where(c => c.LocationId == location.LocationId);
                }
                else
                {
                    return Json(new List<Camera>()); // Return empty list if location not found
                }
            }

            var cameras = await query.ToListAsync();
            return Json(cameras);
        }


        //public async Task<IActionResult> GetCamerasByLocationId(string name, string level)
        //{   
        //    var camerasQuery = _context.Cameras.AsQueryable();

        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        camerasQuery = camerasQuery.Where(c => c.Location.Name.Contains(name));
        //    }
        //    if (!string.IsNullOrEmpty(level))
        //    {
        //        camerasQuery = camerasQuery.Where(c => c.Location.Level.Contains(level));
        //    }

        //    var cameras = await camerasQuery.ToListAsync();

        //    // Return both the filtered cameras and total count
        //    return Json(new { cameras = cameras, totalCount = cameras.Count });
        //}



        //new camera check
        //[HttpPost]
        //[Route("Camera/CheckAllCameras")]
        //public async Task<IActionResult> CheckAllCameras()
        //{
        //    // Fetch all cameras from the database
        //    var cameras = await _context.Cameras.ToListAsync();
        //    if (cameras == null || !cameras.Any())
        //    {
        //        return Json(new { success = false, message = "No cameras found to check." });
        //    }

        //    // Use a background task or Quartz scheduler to handle the process
        //    foreach (var camera in cameras)
        //    {
        //        var jobKey = new JobKey($"CameraCheck_{camera.CameraId}", "CheckAllGroup");
        //        var triggerKey = new TriggerKey($"Trigger_CameraCheck_{camera.CameraId}", "CheckAllGroup");

        //        if (!await _schedulerCameraCheck.CheckExists(jobKey))
        //        {
        //            // Create a job for checking the camera health
        //            IJobDetail job = JobBuilder.Create<AutoCameraCheck>()
        //                .WithIdentity(jobKey)
        //                .UsingJobData("CameraUrl", camera.Url)
        //                .UsingJobData("CameraName", camera.Name)
        //                .UsingJobData("CameraId", camera.CameraId.ToString())
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


        [HttpGet]
        public IActionResult CheckCamera(string cameraUrl, int checkPointId)
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

        //[HttpPost]
        //public async Task<IActionResult> TelegramGroupNotifyAnomaly(string message)
        //{
        //    await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        //[HttpPost]
        //public async Task<IActionResult> TelegramNotifyAnomaly(string message)
        //{
        //    await _telegramService.SendTelegramMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        //[HttpPost]
        //public async Task<IActionResult> SMSNotifyAnomaly(string message)
        //{
        //    await _smsService.SendSMSMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        //[HttpPost]
        //public async Task<IActionResult> SaveCameraCheckResult([FromBody] CameraStatus cameraStatus)
        //{
        //    if (cameraStatus == null)
        //        return BadRequest("No Check Result provided.");

        //    var cameraExists = await _context.Cameras.AnyAsync(c => c.CameraId == cameraStatus.CameraId);
        //    if (!cameraExists)
        //        return BadRequest($"CameraId {cameraStatus.CameraId} does not exist.");

        //    try
        //    {
        //        // Determine status from note
        //        if (cameraStatus.Note.Contains("not working", StringComparison.OrdinalIgnoreCase))
        //        {
        //            cameraStatus.Status = "Not Working";
        //        }
        //        else if (cameraStatus.Note.Contains("working correctly", StringComparison.OrdinalIgnoreCase))
        //        {
        //            cameraStatus.Status = "Working";
        //        }
        //        else if (cameraStatus.Note.Contains("view is blurry", StringComparison.OrdinalIgnoreCase))
        //        {
        //            cameraStatus.Status = "Blurry";
        //        }
        //        else if (cameraStatus.Note.Contains("view is dark", StringComparison.OrdinalIgnoreCase))
        //        {
        //            cameraStatus.Status = "Dark";
        //        }
        //        else
        //        {
        //            cameraStatus.Status = "Unknown"; // Optional fallback
        //        }

        //        cameraStatus.StatusDate = DateTime.Now;
        //        _context.CameraStatuses.Add(cameraStatus);
        //        await _context.SaveChangesAsync();

        //        // Notify only for non-working statuses
        //        if (cameraStatus.Status != "Working")
        //        {
        //            await _telegramService.SendTelegramGroupMessageAsync("6596661148", cameraStatus.Note);
        //            await _telegramService.SendTelegramMessageAsync("60142713849", cameraStatus.Note);
        //            await _smsService.SendSMSMessageAsync("60142713849", cameraStatus.Note);
        //        }

        //        return Ok("Camera check result saved successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error saving camera check result: " + ex.Message);
        //    }
        //}


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

        //[HttpGet]
        //public async Task<IActionResult> GetCameraCheckResults(int page = 1, int pageSize = 20, string cameraName = "", string status = "", DateTime? startDate = null, DateTime? endDate = null)
        //{
        //    try
        //    {
        //        var query = _context.CameraStatuses
        //            .Include(cs => cs.Camera)
        //            .AsQueryable();

        //        // Apply filters
        //        if (!string.IsNullOrEmpty(cameraName))
        //        {
        //            query = query.Where(cs => cs.Camera.Name.Contains(cameraName));
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
        //                CameraId = cs.CameraId,
        //                CameraName = cs.Camera.Name,
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
