using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminPortalV8.Controllers
{
    public class CameraListViewController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        //WebRTC
        //private readonly IList<CameraConfiguration> _cameras;
        //private readonly RTSPtoWebRTCProxyService _webRTCServer;

        public CameraListViewController(UserObj usrObj, IGeneral general, EPatrol_DevContext context)
        {
            //_httpClient = httpClient;
            _usrObj = usrObj;
            _general = general;
            _context = context;
            //_cameras = cameras.Value;
            //_webRTCServer = webRTCServer;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = true;
            try
            {
                var userId = Convert.ToInt32(_usrObj.user.Id);
                var lists = await _general.GetPermissionDefault(userId);
                userId = 0;

                var locations = await _context.Locations
                                              .GroupBy(l => l.Name) // Group by Name to ensure uniqueness
                                              .Select(g => g.First()) // Select the first record in each group
                                              .ToListAsync();

                ViewBag.Locations = locations; // Pass unique locations to the view

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
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


        //public IActionResult CameraFeed()
        //{
        //    // Set the video feed URL dynamically
        //    string videoFeedUrl = "http://192.168.0.2:8888/conference/";

        //    // Pass the URL to the view
        //    ViewBag.VideoFeedUrl = videoFeedUrl;
        //    return View();
        //}

        //[HttpGet]
        //[Route("getcameras")]
        //public IActionResult GetCameras()
        //{
        //    return Ok(_cameras.Select(x => x.Name).ToList());
        //}

        //[HttpGet]
        //[Route("getoffer")]
        //public async Task<IActionResult> GetOffer(string id)
        //{
        //    var camera = _cameras.FirstOrDefault(c => c.Name == id);
        //    if (camera == null)
        //    {
        //        return NotFound("Camera not found.");
        //    }

        //    return Ok(await _webRTCServer.GetOfferAsync(id, camera.Url, camera.UserName, camera.Password));
        //}

        //[HttpPost]
        //[Route("setanswer")]
        //public IActionResult SetAnswer(string id, [FromBody] RTCSessionDescriptionInit answer)
        //{
        //    _webRTCServer.SetAnswer(id, answer);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("addicecandidate")]
        //public IActionResult AddIceCandidate(string id, [FromBody] RTCIceCandidateInit iceCandidate)
        //{
        //    _webRTCServer.AddIceCandidate(id, iceCandidate);
        //    return Ok();
        //}


        //// This endpoint will proxy the video feed from the Flask API
        //[HttpGet("video_feed")]
        //public async Task<IActionResult> VideoFeed()
        //{
        //    // URL of the Flask API serving the video stream
        //    var videoFeedUrl = "http://localhost:5000/video_feed"; // Modify this based on your Flask app's address

        //    // Request the video feed from the Flask API
        //    var stream = await _httpClient.GetStreamAsync(videoFeedUrl);

        //    // Return the video feed as an HTTP response with the correct MIME type
        //    return File(stream, "multipart/x-mixed-replace; boundary=frame");
        //}
    }
}
