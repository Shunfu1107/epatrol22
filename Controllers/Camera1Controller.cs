using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;

namespace AdminPortalV8.Controllers
{
    public class Camera1Controller : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly ITelegramService _telegramService;
        private readonly ISMSService _smsService;
        private readonly IMessageService _messageService;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _schedulerCameraCheck;

        public Camera1Controller(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, ITelegramService telegramService, ISMSService sMSService, IMessageService messageService, ISchedulerFactory schedulerFactory)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _telegramService = telegramService;
            _smsService = sMSService;
            _messageService = messageService;
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
        //public IActionResult GetCameraData(int draw, int start, int length, string search)
        //{
        //    var query = _context.Cameras
        //        .Include(c => c.Location) // ✅ Include Location details
        //        .Select(c => new
        //        {
        //            c.CameraId,
        //            c.Name,
        //            c.Url,
        //            c.CreatedAt,
        //            c.IsActive,
        //            c.IsAutoDetectFire,
        //            CameraImage = c.CameraImage != null ? Convert.ToBase64String(c.CameraImage) : null, // ✅ Convert image to Base64
        //            LocationId = c.LocationId,
        //            LocationName = c.Location != null ? c.Location.Name : null,  // ✅ Fetch Location Name
        //            LocationLevel = c.Location != null ? c.Location.Level : null // ✅ Fetch Location Level
        //        });

        //    // ✅ Apply Search Filter (Ignore Case)
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(c => c.Name.Contains(search) ||
        //                                 c.Url.Contains(search) ||
        //                                 c.CameraId.ToString().Contains(search) ||
        //                                 (c.LocationName != null && c.LocationName.Contains(search)) ||
        //                                 (c.LocationLevel != null && c.LocationLevel.Contains(search)));
        //    }

        //    var totalRecords = query.Count(); // ✅ Get total filtered records

        //    var displayResult = query.OrderBy(c => c.CameraId)  // ✅ Ensure Sorting
        //                              .Skip(start)
        //                              .Take(length)
        //                              .ToList();

        //    return Json(new
        //    {
        //        draw = draw,
        //        recordsTotal = totalRecords,
        //        recordsFiltered = totalRecords, // ✅ Update filtered count
        //        data = displayResult
        //    });
        //}

        [HttpGet]
        public IActionResult GetCameraData(int draw, int start, int length, string search)
        {
            // Retrieve sorting parameters
            int orderColumn = Convert.ToInt32(Request.Query["order[0][column]"]);
            string orderDir = Request.Query["order[0][dir]"]; // "asc" or "desc"

            var query = _context.Cameras
                .Include(c => c.Location)
                .Select(c => new
                {
                    c.CameraId,
                    c.Name,
                    c.Url,
                    c.CreatedAt,
                    c.IsActive,
                    CameraImage = c.CameraImage != null ? Convert.ToBase64String(c.CameraImage) : null,
                    LocationName = c.Location != null ? c.Location.Name : null,
                    LocationLevel = c.Location != null ? c.Location.Level : null
                });

            // ✅ Apply Search Filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) ||
                                         c.Url.Contains(search) ||
                                         c.CameraId.ToString().Contains(search) ||
                                         (c.LocationName != null && c.LocationName.Contains(search)) ||
                                         (c.LocationLevel != null && c.LocationLevel.Contains(search)));
            }

            // ✅ Sorting: Map column index to property name
            query = orderColumn switch
            {
                1 => orderDir == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                2 => query.OrderBy(c => c.CameraId), // Image column: keep non-sortable (front-end should disable)
                3 => orderDir == "asc" ? query.OrderBy(c => c.Url) : query.OrderByDescending(c => c.Url),
                4 => orderDir == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                5 => orderDir == "asc" ? query.OrderBy(c => c.IsActive) : query.OrderByDescending(c => c.IsActive),
                6 => orderDir == "asc" ? query.OrderBy(c => c.LocationName) : query.OrderByDescending(c => c.LocationName),
                7 => orderDir == "asc" ? query.OrderBy(c => c.LocationLevel) : query.OrderByDescending(c => c.LocationLevel),
                _ => query.OrderBy(c => c.CameraId)
            };

            // ✅ Get total records AFTER filtering
            var totalRecords = query.Count();

            // ✅ Apply Pagination
            var displayResult = query.Skip(start).Take(length).ToList();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult
            });
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

        [HttpPost]
        public async Task<IActionResult> AddCamera(string selectedName, string selectedLevel, string Name, string Url, IFormFile CameraImage)
        {
            try
            {
                var existingCamera = await _context.Cameras.FirstOrDefaultAsync(c => c.Url == Url);
                if (existingCamera != null)
                {
                    TempData["ErrorMessage"] = "A camera with this URL already exists.";
                    return RedirectToAction("Index");
                }

                // Find the location based on name and level
                var location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.Name == selectedName && l.Level == selectedLevel);

                byte[] imageData = null;
                if (CameraImage != null && CameraImage.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await CameraImage.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }

                var camera = new Camera
                {
                    Name = Name,
                    Url = Url,
                    LocationId = location?.LocationId,
                    CameraImage = imageData,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Cameras.Add(camera);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to add camera: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //new camera check
        [HttpPost]
        [Route("Camera1/CheckAllCameras")]
        public async Task<IActionResult> CheckAllCameras([FromBody] NotificationRequest request)
        {
            Console.WriteLine($"Received Notification Method: {request.Method}"); // Debugging log

            // Fetch all cameras from the database
            var cameras = await _context.Cameras.ToListAsync();
            if (cameras == null || !cameras.Any())
            {
                return Json(new { success = false, message = "No cameras found to check." });
            }

            // Use a background task or Quartz scheduler to handle the process
            foreach (var camera in cameras)
            {
                var jobKey = new JobKey($"CameraCheck_{camera.CameraId}", "CheckAllGroup");
                var triggerKey = new TriggerKey($"Trigger_CameraCheck_{camera.CameraId}", "CheckAllGroup");

                if (!await _schedulerCameraCheck.CheckExists(jobKey))
                {
                    // Create a job for checking the camera health
                    IJobDetail job = JobBuilder.Create<AutoCameraCheck>()
                        .WithIdentity(jobKey)
                        .UsingJobData("CameraUrl", camera.Url)
                        .UsingJobData("CameraName", camera.Name)
                        .UsingJobData("CameraId", camera.CameraId.ToString())
                        .UsingJobData("notificationMethod", request.Method ?? "unknown")
                        .Build();

                    // Create a simple trigger to fire the job immediately
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .StartNow()
                        .Build();

                    await _schedulerCameraCheck.ScheduleJob(job, trigger);
                }
            }

            return Json(new { success = true, message = "Camera check initiated for all cameras." });
        }

        //[HttpPost]
        //public async Task<IActionResult> TelegramGroupNotifyAnomaly(string message)
        //{
        //    await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        //[HttpPost]
        //public async Task<IActionResult> SMSNotifyAnomaly(string message)
        //{
        //    await _smsService.SendSMSMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        //[HttpPost]
        //public async Task<IActionResult> WhatsAppNotifyAnomaly(string message)
        //{
        //    await _messageService.SendGroupMessageAsync("6596661148", message);
        //    return Ok("Message sent successfully");
        //}

        [HttpGet]
        public async Task<IActionResult> GetCameraCheckResults(int page = 1, int pageSize = 20, string cameraName = "", string status = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.CameraStatusNews
                    .Include(cs => cs.Camera)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(cameraName))
                {
                    query = query.Where(cs => cs.Camera.Name.Contains(cameraName));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(cs => cs.Status == status);
                }
                if (startDate.HasValue)
                {
                    query = query.Where(cs => cs.StatusDate >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(cs => cs.StatusDate <= endDate.Value);
                }

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination and mapping
                var results = await query
                    .OrderByDescending(cs => cs.StatusDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(cs => new
                    {
                        CameraStatusId = cs.CameraStatusId,
                        CameraId = cs.CameraId,
                        CameraName = cs.Camera.Name,
                        Status = cs.Status,
                        Note = cs.Note,
                        Description = cs.Description,
                        NotificationMethod = cs.NotificationMethod,
                        Timestamp = cs.StatusDate.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    results,
                    pagination = new
                    {
                        totalItems,
                        currentPage = page,
                        pageSize,
                        totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving results: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int CameraId, string Name, string Url, IFormFile cameraImage, string selectedName, string selectedLevel)
        {
            var camera = await _context.Cameras.FindAsync(CameraId);

            if (camera == null)
            {
                TempData["ErrorMessage"] = "Camera not found.";
                return RedirectToAction("Index");
            }

            // Update camera properties
            camera.Name = Name;
            camera.Url = Url;

            // Find the location based on name and level
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Name == selectedName && l.Level == selectedLevel);
            camera.LocationId = location?.LocationId;

            // Handle image upload if the user has selected an image
            if (cameraImage != null && cameraImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await cameraImage.CopyToAsync(memoryStream);
                    camera.CameraImage = memoryStream.ToArray(); // Save the image as VARBINARY
                }
            }

            // Save changes to the database
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
    }
}