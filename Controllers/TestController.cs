using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AdminPortalV8.Controllers
{
    public class TestController : Controller
    {
        private readonly EPatrol_DevContext _context;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;

        public TestController(EPatrol_DevContext context, UserObj usrObj, IGeneral general)
        {
            _context = context;
            _usrObj = usrObj;
            _general = general;
        }

        [HttpPost]
        public IActionResult Create(CameraStatus cameraStatus)
        {
            if (ModelState.IsValid)
            {
                _context.CameraStatuses.Add(cameraStatus);
                _context.SaveChanges();
                return Json(new { success = true, message = "Record successfully added" });
            }

            return Json(new { success = false, message = "Error occurred while saving" });
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upload(IFormFile video)
        {
            if (video == null || video.Length == 0)
            {
                return BadRequest(new { success = false, message = "没有选择视频文件" });
            }

            try
            {
                byte[] videoData;
                using (var memoryStream = new MemoryStream())
                {
                    await video.CopyToAsync(memoryStream);
                    videoData = memoryStream.ToArray();
                }

                // Set content type based on file extension
                string contentType = video.ContentType; // Default from the file upload
                if (string.IsNullOrEmpty(contentType))
                {
                    // Infer content type based on file extension if not provided
                    var extension = Path.GetExtension(video.FileName).ToLower();
                    if (extension == ".mp4")
                    {
                        contentType = "video/mp4";
                    }
                    else if (extension == ".avi")
                    {
                        contentType = "video/x-msvideo";
                    }
                    else
                    {
                        contentType = "application/octet-stream"; // Fallback for unsupported types
                    }
                }

                var videoFile = new VideoFile
                {
                    FileName = video.FileName,
                    Content = videoData,
                    ContentType = contentType
                };

                _context.VideoFiles.Add(videoFile);
                await _context.SaveChangesAsync();

                // Redirect to Index after successful upload
                return RedirectToAction("Index", "Test");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"视频上传失败: {ex.Message}" });
            }
        }


        public async Task<IActionResult> PlayVideo(int id)
        {
            // Retrieve the video file data
            var videoFile = await _context.VideoFiles.FindAsync(id);

            // Debugging: Check if the video is found
            if (videoFile == null)
            {
                return NotFound(); // If the video is not found, return 404
            }

            // Debugging: Check if content is empty or invalid
            if (videoFile.Content == null || videoFile.Content.Length == 0)
            {
                return BadRequest("Video content is empty for ID " + id); // Return an error if content is empty
            }

            // Debugging: Log the content type being returned
            Console.WriteLine($"Returning video with ContentType: {videoFile.ContentType}");

            // Return the video content as a file response
            return File(videoFile.Content, videoFile.ContentType);
        }

    }
}
