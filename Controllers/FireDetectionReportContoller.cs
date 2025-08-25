using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Web.WebPages;

namespace AdminPortalV8.Controllers
{
    public class FireDetectionReportController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IEncryption _encryptionService;


        public FireDetectionReportController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IEncryption encryptionService)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);

                var lists = await _general.GetPermissionDefault(userId);

                userId = 0;

                var camera = await _context.Cameras.ToListAsync();
                ViewBag.Cameras = camera;

                var fireDetected = await _context.FireDetecteds.ToListAsync();
                ViewBag.FireDetecteds = fireDetected;

                var fireTypes = fireDetected.Select(f => f.Type).Distinct().ToList();
                ViewBag.FireTypes = fireTypes;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> FireDetectionReportTable(string cameraName, string dateRange, string startTime, string endTime, string typeFilter)
        {
            try
            {
                var dateParts = dateRange.Split(new[] { " to " }, StringSplitOptions.None);
                var startDateParsed = DateOnly.Parse(dateParts[0].Trim());
                var endDateParsed = DateOnly.Parse(dateParts[1].Trim());
                var startTimeParsed = TimeOnly.Parse(startTime);
                var endTimeParsed = TimeOnly.Parse(endTime);

                var reports = await (from fireDetected in _context.FireDetecteds
                                     join camera in _context.Cameras on fireDetected.CameraId equals camera.CameraId
                                     where camera.Name == cameraName &&
                                           fireDetected.CreatedAt.HasValue && // Check if CreatedAt is not null
                                           fireDetected.CreatedAt.Value.Date >= startDateParsed.ToDateTime(TimeOnly.MinValue) &&
                                           fireDetected.CreatedAt.Value.Date <= endDateParsed.ToDateTime(TimeOnly.MaxValue) &&
                                           fireDetected.CreatedAt.Value.TimeOfDay >= startTimeParsed.ToTimeSpan() &&
                                           fireDetected.CreatedAt.Value.TimeOfDay <= endTimeParsed.ToTimeSpan() &&
                                           (string.IsNullOrEmpty(typeFilter) || fireDetected.Type == typeFilter)  // <-- Type filter
                                     select new
                                     {
                                         cameraName = camera.Name,
                                         videoPath = fireDetected.VideoPath,
                                         date = fireDetected.CreatedAt.HasValue ? fireDetected.CreatedAt.Value.ToString("dd/MM/yyyy") : null,
                                         //date = fireDetected.CreatedAt.Value.ToString("dd/MM/yyyy"),
                                         time = fireDetected.CreatedAt.Value.ToString("HH:mm:ss"),
                                         type = fireDetected.Type,  // <-- Added Type
                                         videoLink = fireDetected.VideoBin != null
                                             ? $"data:video/mp4;base64,{Convert.ToBase64String(fireDetected.VideoBin)}"
                                             : null
                                     })
                           .ToListAsync();

                return Json(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public async Task<IActionResult> PlayVideo(int id)
        {
            // Retrieve the video file data
            var videoFile = await _context.VideoFiles.FindAsync(id);

            if (videoFile == null || videoFile.Content == null || videoFile.Content.Length == 0)
            {
                return NotFound("Video not found or content is empty.");
            }

            // Convert the binary data to Base64
            var base64Video = Convert.ToBase64String(videoFile.Content);

            // Return the Base64 data as a JSON object with the MIME type
            return Json(new
            {
                mimeType = videoFile.ContentType, // E.g., "video/mp4"
                base64Data = base64Video
            });
        }


    }
}
