using System.Globalization;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Controllers
{
    public class FireDetectionReport2Controller : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;

        public FireDetectionReport2Controller(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                // Check if userId is null and handle accordingly
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
                // Handle invalid user ID format
                Console.WriteLine($"FormatException: {ex.Message}");
                return BadRequest("Invalid user ID format.");
            }
            catch (Exception ex)
            {
                // Catch any other exception and log it
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public IActionResult GetAiModels()
        {
            var models = _context.CheckLists
                .Select(m => new {
                    id = m.CheckListId,
                    name = m.CheckListName
                }).ToList();

            return Json(models); // still returns JSON, but wrapped in IActionResult
        }

        [HttpGet]
        public IActionResult GetCameras()
        {
            var cameras = _context.Cameras
                .Select(c => new { cameraId = c.CameraId, name = c.Name })
                .ToList();

            return Json(cameras);
        }

        [HttpGet]
        public IActionResult GetReportTypes()
        {
                var reportTypes = _context.FireDetecteds
                                         .Select(x => x.Type)
                                         .Distinct()
                                         .OrderBy(x => x)
                                         .ToList();

                return Json(reportTypes);
        }


        [HttpGet]
        public IActionResult GetPatrolData(int cameraId, string type, string dateRange, string startTime, string endTime)
        {
            try
            {
                // Split date range into start and end dates
                var dateParts = dateRange.Split(new[] { " to " }, StringSplitOptions.None);
                var startDate = DateOnly.Parse(dateParts[0].Trim());
                var endDate = DateOnly.Parse(dateParts[1].Trim());

                TimeOnly startTimeSpan = TimeOnly.Parse(startTime);
                TimeOnly endTimeSpan = TimeOnly.Parse(endTime);

                var query = _context.FireDetecteds
                    .Include(p => p.Camera)
                    .Where(p => p.CameraId == cameraId &&
                                p.Type == type &&
                                p.CreatedAt.HasValue &&
                                p.CreatedAt.Value.Date >= startDate.ToDateTime(TimeOnly.MinValue) &&
                                p.CreatedAt.Value.Date <= endDate.ToDateTime(TimeOnly.MaxValue) &&
                                p.CreatedAt.Value.TimeOfDay >= startTimeSpan.ToTimeSpan() &&
                                p.CreatedAt.Value.TimeOfDay <= endTimeSpan.ToTimeSpan());

                var data = query.Select(p => new
                {
                    cameraName = p.Camera.Name,
                    videoPath = p.VideoPath,
                    date = p.CreatedAt.HasValue ? p.CreatedAt.Value.ToString("dd/MM/yyyy") : null,
                    time = p.CreatedAt.HasValue ? p.CreatedAt.Value.ToString("HH:mm:ss") : null,
                    type = p.Type,
                    videoLink = p.VideoBin != null ? $"data:video/mp4;base64,{Convert.ToBase64String(p.VideoBin)}" : null
                }).ToList();

                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching patrol data.", details = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> PlayVideo(int id)
        {
            var videoFile = await _context.VideoFiles.FindAsync(id);

            if (videoFile == null || videoFile.Content == null || videoFile.Content.Length == 0)
            {
                return NotFound("Video not found or content is empty.");
            }

            var base64Video = Convert.ToBase64String(videoFile.Content);

            return Json(new
            {
                mimeType = videoFile.ContentType,
                base64Data = base64Video
            });
        }

        //[HttpGet]
        //public IActionResult GetPatrolData(int type, int routeId, string checkPointName, string dateRange, string startTime, string endTime)
        //{
        //    try
        //    {
        //        // Parse date range
        //        var dateParts = dateRange.Split(new[] { " to " }, StringSplitOptions.None);
        //        var startDate = DateOnly.Parse(dateParts[0].Trim());
        //        var endDate = DateOnly.Parse(dateParts[1].Trim());

        //        TimeOnly startTimeSpan = TimeOnly.Parse(startTime);
        //        TimeOnly endTimeSpan = TimeOnly.Parse(endTime);

        //        // Fetch patrol data from database
        //        var patrolData = _context.Patrols
        //            .Where(p => p.RouteId == routeId &&
        //                       p.CheckPointName == checkPointName &&
        //                       p.Date >= startDate &&
        //                       p.Date <= endDate &&
        //                       p.Time >= startTimeSpan &&
        //                       p.Time <= endTimeSpan)
        //            .Select(p => new
        //            {
        //                p.PatrolId,
        //                p.Route.RouteName,
        //                p.CheckPointName,
        //                p.Date,
        //                p.Time,
        //                p.CheckListName,
        //                p.Status
        //            })
        //            .ToList();

        //        return Json(patrolData);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "An error occurred while fetching patrol data.", details = ex.Message });
        //    }
        //}

        //[HttpGet]
        //public IActionResult GetPatrolData(int draw, int start, int length, string search)
        //{
        //    // Retrieve sorting parameters
        //    int orderColumn = Convert.ToInt32(Request.Query["order[0][column]"]);
        //    string orderDir = Request.Query["order[0][dir]"]; // "asc" or "desc"


        //    var query = _context.Patrols
        //        .Include(p => p.Guard)         // Include related Guard data
        //        .Include(p => p.Reports)       // Include related Reports data
        //        .Include(p => p.Route)         // Include related Route data
        //        .Where(p => p.Date.HasValue && p.Date.Value == todayDate) // ✅ Filter by today's patrols
        //        .Select(p => new
        //        {
        //            p.PatrolId,
        //            PatrolDate = p.Date.HasValue ? p.Date.Value.ToString("yyyy-MM-dd") : null,
        //            PatrolTime = p.Time.HasValue ? p.Time.Value.ToString("HH:mm") : null,
        //            p.Status,
        //            p.RouteId,
        //            RouteName = p.Route != null ? p.Route.RouteName : null,
        //            p.Note,
        //            p.CheckListName,
        //            p.CheckPointName,
        //            p.Link,
        //            GuardName = p.Guard != null ? p.Guard.Name : null,
        //            ReportCount = p.Reports.Count,
        //            Video = p.VideoLink != null ? Convert.ToBase64String(p.VideoLink) : null
        //        });

        //    // ✅ Apply Search Filter
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(p => p.Status.Contains(search) ||
        //                                 p.RouteId.ToString().Contains(search) ||
        //                                 p.CheckPointName.Contains(search));
        //                                 //p.Guard.Name.Contains(search));
        //    }

        //    // ✅ Sorting: Map column index to property name
        //    query = orderColumn switch
        //    {
        //        1 => orderDir == "asc" ? query.OrderBy(p => p.PatrolDate) : query.OrderByDescending(p => p.PatrolDate),
        //        2 => orderDir == "asc" ? query.OrderBy(p => p.PatrolTime) : query.OrderByDescending(p => p.PatrolTime),
        //        3 => orderDir == "asc" ? query.OrderBy(p => p.Status) : query.OrderByDescending(p => p.Status),
        //        _ => query.OrderBy(p => p.PatrolId) // Default sorting
        //    };

        //    // ✅ Get total records AFTER filtering
        //    var totalRecords = query.Count();

        //    // ✅ Apply Pagination
        //    var displayResult = query.Skip(start).Take(length).ToList();

        //    return Json(new
        //    {
        //        draw = draw,
        //        recordsTotal = totalRecords,
        //        recordsFiltered = totalRecords,
        //        data = displayResult
        //    });
        //}
    }
}
