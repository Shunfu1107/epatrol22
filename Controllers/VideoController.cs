using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Controllers
{
    public class VideoController : Controller
    {

        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;

        public VideoController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);

                userId = 0; // Please comment if using restaurant filter

                // Fetch video records
                var videos = await _context.Videos.ToListAsync();

                // Encrypt the video paths
                foreach (var video in videos)
                {
                    video.VideoPath = PathEncryptor.Encrypt(video.VideoPath);
                }

                ViewBag.Video = videos;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult StreamVideo(string path)
        {
            try
            {
                // Decrypt the path before using it
                string decryptedPath = PathEncryptor.Decrypt(path);

                if (System.IO.File.Exists(decryptedPath))
                {
                    var stream = new FileStream(decryptedPath, FileMode.Open, FileAccess.Read);
                    var mimeType = "video/mp4"; // Adjust based on your video type
                    return File(stream, mimeType);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
