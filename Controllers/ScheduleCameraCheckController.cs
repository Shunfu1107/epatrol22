using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using iText.Commons.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz;
using EPatrol.Services;
using Google.Apis.Util;
using System.Web.WebPages;
using Quartz.Impl.Matchers;
using Syncfusion.UI.Xaml.Charts;

namespace AdminPortalV8.Controllers
{
    public class NotificationRequest
    {
        public string Method { get; set; }
    }

    public class ScheduleCameraCheckController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _schedulerCameraCheck;

        public ScheduleCameraCheckController(UserObj usrObj, IGeneral general, EPatrol_DevContext ePatrol_DevContext, ISchedulerFactory schedulerFactory)
        {
            _general = general;
            _usrObj = usrObj;
            _context = ePatrol_DevContext;
            _schedulerFactory = schedulerFactory;
            _schedulerCameraCheck = _schedulerFactory.GetScheduler().Result;
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var locations = _context.Locations.ToList(); // Fetch data from the database
        //    //var locationCameras = _context.LocationCameras.ToList();
        //    var cameras = _context.Cameras.ToList();
        //    ViewBag.LocationNames = locations.Select(l => l.Name).Distinct().ToList();
        //    ViewBag.LocationLevels = locations.Select(l => l.Level).Distinct().ToList();

        //    ViewBag.Cameras = cameras;

        //    DateTime timelocal = DateTime.Now;
        //    Console.WriteLine("@@@@@@@@@@@@@@@");
        //    Console.WriteLine(timelocal);
        //    Console.WriteLine("@@@@@@@@@@@@@@@@");

        //    return View();
        //}


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

        [HttpGet]
        public IActionResult GetCameraData(int draw, int start, int length, string search)
        {
            // Retrieve sorting parameters
            int orderColumn = Convert.ToInt32(Request.Query["order[0][column]"]);
            string orderDir = Request.Query["order[0][dir]"]; // "asc" or "desc"

            var query = _context.Cameras
                .Include(c => c.Location) // ✅ Include Location details
                .Select(c => new
                {
                    c.CameraId,
                    c.Name,
                    c.Url,
                    c.CreatedAt,
                    c.IsActive,
                    c.IsAutoDetectFire,
                    CameraImage = c.CameraImage != null ? Convert.ToBase64String(c.CameraImage) : null, // ✅ Convert image to Base64
                    LocationId = c.LocationId,
                    LocationName = c.Location != null ? c.Location.Name : null,  // ✅ Fetch Location Name
                    LocationLevel = c.Location != null ? c.Location.Level : null // ✅ Fetch Location Level
                });

            // ✅ Apply Search Filter (Ignore Case)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) ||
                                         c.Url.Contains(search) ||
                                         c.CameraId.ToString().Contains(search) ||
                                         (c.LocationName != null && c.LocationName.Contains(search)) ||
                                         (c.LocationLevel != null && c.LocationLevel.Contains(search)));
            }

            query = orderColumn switch
            {
                1 => orderDir == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                2 => orderDir == "asc" ? query.OrderBy(c => c.Url) : query.OrderByDescending(c => c.Url),
                3 => orderDir == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                4 => orderDir == "asc" ? query.OrderBy(c => c.IsActive) : query.OrderByDescending(c => c.IsActive),
                5 => orderDir == "asc" ? query.OrderBy(c => c.LocationName) : query.OrderByDescending(c => c.LocationName),
                6 => orderDir == "asc" ? query.OrderBy(c => c.LocationLevel) : query.OrderByDescending(c => c.LocationLevel),
                _ => query.OrderBy(c => c.CameraId) // Default sorting
            };

            // ✅ Get total records AFTER filtering
            var totalRecords = query.Count();


            // ✅ Apply Pagination
            var displayResult = query.Skip(start).Take(length).ToList();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords, // ✅ Update filtered count
                data = displayResult
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetCameraSchedules(int cameraId)
        {
            try
            {
                var schedules = await _context.ScheduleCameraChecks
                    .Where(s => s.Cameras.Any(c => c.CameraId == cameraId))
                    .Select(s => new
                    {
                        s.ScheduleId,
                        ScheduleName = "Health Check Schedule",
                        StartTime = s.StartTime.ToString("HH:mm"),
                        s.NotificationMethod
                    })
                    .ToListAsync();

                return Json(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error fetching schedules: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetCameraHelthCheckSchedule(string CameraId, string Time, string NotificationMethod)
        {
            try
            {
                var camera = await _context.Cameras
                    .Include(c => c.Location)
                    .FirstOrDefaultAsync(c => c.CameraId.ToString() == CameraId);

                if (camera == null)
                {
                    return Json(new { success = false, message = "Camera not found" });
                }

                // Handle location name and level return null value
                string locationName = camera.Location?.Name ?? "Unknown";
                string level = camera.Location?.Level ?? "Unknown";

                Console.WriteLine("result " + locationName, level);

                var cronScheduleExp = TimeDayConverter.Time24Converter(Time);
                var jobKey = new JobKey(CameraId, cronScheduleExp);
                var triggerKey = new TriggerKey(CameraId, cronScheduleExp);

                if (await _schedulerCameraCheck.CheckExists(jobKey))
                {
                    await _schedulerCameraCheck.DeleteJob(jobKey);
                }

                // Create and schedule the job
                IJobDetail job = JobBuilder.Create<AutoCamHealthCheckJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("CameraUrl", camera.Url)
                    .UsingJobData("CameraName", camera.Name)
                    .UsingJobData("CameraId", CameraId)
                    .UsingJobData("NotificationMethod", NotificationMethod)
                    .UsingJobData("BlockName", locationName) // Pass Location Name
                    .UsingJobData("Level", level)           // Pass Level
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithCronSchedule(cronScheduleExp)
                    .Build();

                await _schedulerCameraCheck.ScheduleJob(job, trigger);

                // Add new schedule to database
                var newSchedule = new ScheduleCameraCheck
                {
                    ScheduleName = "Health Check Schedule",
                    StartTime = TimeOnly.Parse(Time),
                    Day = "EVERYDAY",
                    NotificationMethod = NotificationMethod
                };

                newSchedule.Cameras = new List<Camera> { camera };
                _context.ScheduleCameraChecks.Add(newSchedule);
                await _context.SaveChangesAsync();
                Console.WriteLine("Schedule successfully saved to database");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCameraSchedule(int scheduleId, int cameraId, string scheduleTime, string notificationMethod)
        {

            try
            {
                Console.WriteLine($"Deleting Schedule: ID={scheduleId}, CameraID={cameraId}, Time={scheduleTime}, Notification={notificationMethod}");

                // Code to delete the schedule from the database
                // Find the schedule by its ID
                var schedule = _context.ScheduleCameraChecks
                    .Include(s => s.Cameras) // Load related Cameras
                    .AsSplitQuery()
                    .FirstOrDefault(s => s.ScheduleId == scheduleId);

                if (schedule == null)
                {
                    return NotFound(); // Return 404 if not found
                }

                // Clear the relationship with Cameras (this removes entries in the intermediate table)
                schedule.Cameras.Clear();
                _context.SaveChanges(); // Save to update the intermediate table

                // Now remove the ScheduleCameraCheck entity
                _context.ScheduleCameraChecks.Remove(schedule);
                await _context.SaveChangesAsync(); // Save final changes

                var schedules = await _context.ScheduleCameraChecks
                    .Where(s => s.Cameras.Any(c => c.CameraId == cameraId))
                    .Select(s => new
                    {
                        s.ScheduleId,
                        s.ScheduleName,
                        s.StartTime,
                        s.NotificationMethod
                    })
                    .ToListAsync();

                Console.WriteLine($"Deleted schedule with Notification Method: {notificationMethod}");


                // remove schedule
                string fullTime = scheduleTime;
                string shortTime = fullTime.Substring(0, 5); // "16:32"

                var cronScheduleExp = TimeDayConverter.Time24Converter(shortTime);
                var key_name = cameraId.ToString();
                var group_name = cronScheduleExp.ToString();
                var jobKey = new JobKey(key_name, group_name);

                // Check if the job already exists
                if (await _schedulerCameraCheck.CheckExists(jobKey))
                {
                    await _schedulerCameraCheck.DeleteJob(jobKey); // Delete the existing job
                }


                return Json(new { success = true, schedules = schedules });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCameraCheckResults(int page = 1, int pageSize = 20, string cameraName = "", string status = "", string blockName = "", string level = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.CameraHealthResults
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(cameraName))
                {
                    query = query.Where(cs => cs.CameraName.Contains(cameraName));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(cs => cs.Status == status);
                }
                if (!string.IsNullOrEmpty(blockName))
                {
                    query = query.Where(cs => cs.BlockName == blockName);
                }
                if (!string.IsNullOrEmpty(level))
                {
                    query = query.Where(cs => cs.Level == level);
                }
                if (startDate.HasValue)
                {
                    query = query.Where(cs => cs.Timestamp >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(cs => cs.Timestamp <= endDate.Value);
                }

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var results = await query
                    .OrderBy(cs => cs.Timestamp) // Order by timestamp
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(cs => new
                    {
                        CameraId = cs.CameraId,
                        CameraName = cs.CameraName,
                        Status = cs.Status,
                        Message = cs.Result,
                        BlockName = cs.BlockName,
                        Level = cs.Level,
                        Timestamp = cs.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
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
                return Json(new { success = false, message = "Error retrieving results: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Method))
            {
                return Json(new { success = false, message = "Notification method is required." });
            }

            string method = request.Method;
            Console.WriteLine($"Notification method selected: {method}");

            // Fetch all cameras from the database
            var cameras = await _context.Cameras.ToListAsync();
            if (cameras == null || !cameras.Any())
            {
                return Json(new { success = false, message = "No cameras found to send notifications." });
            }

            // Use a background task or Quartz scheduler to handle the process
            foreach (var camera in cameras)
            {
                var jobKey = new JobKey($"Notification_{camera.CameraId}", "NotificationGroup");
                var triggerKey = new TriggerKey($"Trigger_Notification_{camera.CameraId}", "NotificationGroup");

                if (!await _schedulerCameraCheck.CheckExists(jobKey))
                {
                    // Create a job for sending the notification
                    IJobDetail job = JobBuilder.Create<AutoCamHealthCheckJob>()
                        .WithIdentity(jobKey)
                        .UsingJobData("CameraUrl", camera.Url)
                        .UsingJobData("CameraName", camera.Name)
                        .UsingJobData("CameraId", camera.CameraId.ToString())
                        .UsingJobData("NotificationMethod", method)
                        .Build();

                    // Create a simple trigger to fire the job immediately
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .StartNow()
                        .Build();

                    await _schedulerCameraCheck.ScheduleJob(job, trigger);
                }
            }

            return Json(new { success = true, message = "Notification initiated for all cameras." });
        }


        [HttpGet]
        public async Task<IActionResult> GetFilterOptions()
        {
            try
            {
                // Fetch unique block names and levels from the Location table
                var blockNames = await _context.Locations
                    .Select(l => l.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToListAsync();

                var levels = await _context.Locations
                    .Select(l => l.Level)
                    .Distinct()
                    .OrderBy(level => level)
                    .ToListAsync();

                return Json(new { success = true, blockNames, levels });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving filter options: " + ex.Message });
            }
        }
    }
}