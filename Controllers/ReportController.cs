using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web.WebPages;

namespace AdminPortalV8.Controllers
{
    public class ReportController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IEncryption _encryptionService;


        public ReportController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IEncryption encryptionService)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _encryptionService =  encryptionService;
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

                var route = await _context.Routes.ToListAsync();
                ViewBag.Routes = route;

                var checkpoint = await _context.CheckPoints.ToListAsync();
                ViewBag.CheckPoints = checkpoint;

                var patrol = await _context.Patrols.ToListAsync();
                ViewBag.Patrols = patrol;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> ReportTable(int routeId, string checkPointName, string dateRange, int type, string startTime, string endTime)
        {
            try
            {
                var dateParts = dateRange.Split(new[] { " to " }, StringSplitOptions.None);
                var startDate = DateOnly.Parse(dateParts[0].Trim());
                var endDate = DateOnly.Parse(dateParts[1].Trim());

                TimeOnly startTimeSpan = TimeOnly.Parse(startTime);
                TimeOnly endTimeSpan = TimeOnly.Parse(endTime);

                var reports = await (from patrol in _context.Patrols
                                     join route in _context.Routes on patrol.RouteId equals route.RouteId
                                     //join guard in _context.Guards on patrol.GuardId equals guard.GuardId into guardGroup
                                     //from guard in guardGroup.DefaultIfEmpty() // Left join for guard
                                     where patrol.RouteId == routeId &&
                                           patrol.CheckPointName.Contains(checkPointName) &&
                                           route.PatrolTypeId == type &&
                                           patrol.Date >= startDate && patrol.Date <= endDate &&
                                           patrol.Time >= startTimeSpan && patrol.Time <= endTimeSpan

                                     select new
                                     {
                                         routeName = route.RouteName,
                                         checkPointName = patrol.CheckPointName,
                                         date = patrol.Date.HasValue ? patrol.Date.Value.ToString("dd/MM/yyyy") : null,
                                         time = string.Format("{0:hh\\:mm\\:ss}", patrol.Time),
                                         checkList = patrol.CheckListName,
                                         status = patrol.Status,
                                         patrolTypeId = route.PatrolTypeId,
                                         emergency = patrol.Note,
                                         guard = patrol.GuardName, // Handle null guard
                                                                                    // Decrypt the link before returning it
                                                                                    //link = _encryptionService.Decrypt(patrol.Link)
                                         link = patrol.VideoLink != null
                                            ? $"data:video/mp4;base64,{Convert.ToBase64String(patrol.VideoLink)}"
                                            : null
                                         //link = patrol.Link
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
