using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using AdminPortalV8.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Diagnostics;

namespace AdminPortalV8.Controllers
{
    public class FireDetection24hrsController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly ITelegramService _telegramService;
        private readonly ISMSService _smsService;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _schedulerCameraCheck;

        public FireDetection24hrsController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, ITelegramService telegramService, ISMSService sMSService, ISchedulerFactory schedulerFactory)
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
                var checklists = await _context.CheckLists.ToListAsync();
                var cameraChecklists = await _context.CameraCheckLists.ToListAsync();

                // Pass the cameras and locations to the view
                ViewBag.Cameras = cameras;
                ViewBag.Locations = locations;
                ViewBag.CheckLists = checklists;
                ViewBag.CameraCheckLists = cameraChecklists;

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
            // Step 1: Load all required data first (materialize the query)
            var allData = _context.Cameras
                .Include(c => c.Location)
                .Include(c => c.CameraCheckLists)
                    .ThenInclude(cc => cc.CheckList)
                .ToList(); // Bring everything to memory

            // Step 2: Apply search in-memory
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();

                allData = allData.Where(c =>
                    (c.Name?.ToLower().Contains(lowerSearch) ?? false) ||
                    (c.Url?.ToLower().Contains(lowerSearch) ?? false) ||
                    c.CameraId.ToString().Contains(lowerSearch) ||
                    (c.Location?.Name?.ToLower().Contains(lowerSearch) ?? false) ||
                    (c.Location?.Level?.ToLower().Contains(lowerSearch) ?? false) ||
                    c.CameraCheckLists.Any(cc =>
                        (cc.CheckList?.CheckListName?.ToLower().Contains(lowerSearch) ?? false) ||
                        (cc.StartTime.HasValue && cc.StartTime.Value.ToString(@"hh\:mm").ToLower().Contains(lowerSearch)) ||
                        (cc.EndTime.HasValue && cc.EndTime.Value.ToString(@"hh\:mm").ToLower().Contains(lowerSearch)))
                ).ToList();
            }

            var totalRecords = allData.Count;

            // Step 3: Paginate after filtering
            var pagedData = allData
                .OrderBy(c => c.CameraId)
                .Skip(start)
                .Take(length)
                .ToList();

            // Step 4: Project to return shape
            var displayResult = pagedData.Select(c => new
            {
                c.CameraId,
                c.Name,
                c.Url,
                c.IsActive,
                CameraImage = c.CameraImage != null ? Convert.ToBase64String(c.CameraImage) : null,
                LocationId = c.LocationId,
                LocationName = c.Location?.Name,
                LocationLevel = c.Location?.Level,
                Checklists = c.CameraCheckLists.Select(cc => new
                {
                    Name = cc.CheckList?.CheckListName,
                    StartTime = cc.StartTime?.ToString(@"hh\:mm"),
                    EndTime = cc.EndTime?.ToString(@"hh\:mm")
                }).ToList()
            });

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

        [HttpGet]
        public async Task<IActionResult> GetCameraCheckLists(int cameraId)
        {
            try
            {
                // Fetch checklists associated with the camera (existing logic)
                var cameraChecklists = await (from cameraChecklist in _context.CameraCheckLists
                                              join checklist in _context.CheckLists on cameraChecklist.CheckListId equals checklist.CheckListId into checkListGroup
                                              from checklist in checkListGroup.DefaultIfEmpty()
                                              where cameraChecklist.CameraId == cameraId
                                              select new
                                              {
                                                  cameraChecklist.CheckListId,
                                                  StartTime = cameraChecklist.StartTime.HasValue
                                                      ? cameraChecklist.StartTime.Value.ToString(@"hh\:mm\:ss")
                                                      : null,
                                                  EndTime = cameraChecklist.EndTime.HasValue
                                                      ? cameraChecklist.EndTime.Value.ToString(@"hh\:mm\:ss")
                                                      : null,
                                                  CheckListName = checklist != null ? checklist.CheckListName : null
                                              })
                                             .ToListAsync();

                // Fetch all available checklists from the CheckLists table
                var allChecklists = await _context.CheckLists
                    .Select(cl => new
                    {
                        cl.CheckListId,
                        cl.CheckListName
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    cameraChecklists, // Checklists associated with the camera
                    allChecklists     // All available checklists for the dropdown
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpGet]
        //public IActionResult GetCameraCheckLists(int cameraId)
        //{
        //    var checklists = _context.CameraCheckLists
        //        .Where(cc => cc.CameraId == cameraId)
        //        .Select(cc => new
        //        {
        //            cc.CheckListId,
        //            StartTime = cc.StartTime.HasValue ? cc.StartTime.Value.ToString(@"hh\:mm\:ss") : null,
        //            EndTime = cc.EndTime.HasValue ? cc.EndTime.Value.ToString(@"hh\:mm\:ss") : null,
        //            CheckListName = _context.CheckLists
        //                .Where(cl => cl.CheckListId == cc.CheckListId)
        //                .Select(cl => cl.CheckListName)
        //                .FirstOrDefault()
        //        }).ToList();


        //    if (checklists != null && checklists.Count > 0)
        //    {
        //        return Json(new { success = true, checklists });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "No checklists found for this camera." });
        //    }
        //}

        [HttpPost]
        public IActionResult UpdateCameraChecklist([FromBody] UpdateChecklistRequest request)
        {
            try
            {
                var existingChecklists = _context.CameraCheckLists
                    .Where(cc => cc.CameraId == request.CameraId)
                    .ToList();

                // ✅ Update or delete existing checklists
                foreach (var cc in existingChecklists)
                {
                    var updatedChecklist = request.Checklists.FirstOrDefault(c => c.CheckListId == cc.CheckListId);
                    if (updatedChecklist != null)
                    {
                        cc.StartTime = TimeSpan.TryParse(updatedChecklist.StartTime, out var startTime) ? startTime : (TimeSpan?)null;
                        cc.EndTime = TimeSpan.TryParse(updatedChecklist.EndTime, out var endTime) ? endTime : (TimeSpan?)null;
                    }
                    else
                    {
                        _context.CameraCheckLists.Remove(cc); // ✅ Delete checklists that are no longer in UI
                    }
                }

                // ✅ Add new checklists
                foreach (var newChecklist in request.Checklists.Where(c => !existingChecklists.Any(ec => ec.CheckListId == c.CheckListId)))
                {
                    _context.CameraCheckLists.Add(new Camera_CheckList
                    {
                        CameraId = request.CameraId,
                        CheckListId = newChecklist.CheckListId,
                        StartTime = TimeSpan.TryParse(newChecklist.StartTime, out var startTime) ? startTime : (TimeSpan?)null,
                        EndTime = TimeSpan.TryParse(newChecklist.EndTime, out var endTime) ? endTime : (TimeSpan?)null
                    });
                }

                _context.SaveChanges();
                return Json(new { success = true, message = "Checklist updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating checklists: {ex.Message}");
                return Json(new { success = false, message = "Error updating checklists: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFireDetection([FromBody] FireDetectionUpdateModel model)
        {
            if (model == null || model.CameraId == 0)
                return Json(new { success = false, message = "Invalid data received." });

            var camera = await _context.Cameras.FindAsync(model.CameraId);
            if (camera == null)
                return Json(new { success = false, message = "Camera not found." });

            bool wasEnabled = camera.IsAutoDetectFire;
            camera.IsAutoDetectFire = model.IsAutoDetectFire == 1;
            await _context.SaveChangesAsync();

            if (!wasEnabled && camera.IsAutoDetectFire)
            {
                StartFireDetection(camera.Url);
            }
            else if (wasEnabled && !camera.IsAutoDetectFire)
            {
                StopFireDetection(camera.Url);
            }

            return Json(new { success = true });
        }

        private void StartFireDetection(string cameraUrl)
        {
            try
            {
                string pythonExe = Path.Combine(Directory.GetCurrentDirectory(), "fire_detection", ".venv", "Scripts", "python.exe");
                string scriptDir = Path.Combine(Directory.GetCurrentDirectory(), "fire_detection");
                string script = "fire_detect.py";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{Path.Combine(scriptDir, script)}\" --camera \"{cameraUrl}\"",
                    WorkingDirectory = scriptDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(psi);
                Console.WriteLine($"🔥 Fire Detection started for {cameraUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error starting Fire Detection: {ex.Message}");
            }
        }

        private void StopFireDetection(string cameraUrl)
        {
            try
            {
                var pythonProcesses = Process.GetProcessesByName("python");
                foreach (var process in pythonProcesses)
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                Console.WriteLine($"🛑 Fire Detection stopped for {cameraUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error stopping Fire Detection: {ex.Message}");
            }
        }

        public class FireDetectionUpdateModel
        {
            public int CameraId { get; set; }
            public int IsAutoDetectFire { get; set; } // 1 for true, 0 for false
        }

        public class UpdateChecklistRequest
        {
            public int CameraId { get; set; }
            public List<ChecklistData> Checklists { get; set; }
        }

        public class ChecklistData
        {
            public int CheckListId { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
    }
}