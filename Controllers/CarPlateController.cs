using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;

namespace AdminPortalV8.Controllers
{
    public class CarPlateController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        public CarPlateController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);
                userId = 0;

                var cameras = await _context.Cameras.ToListAsync();
                ViewBag.Cameras = cameras;

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
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
