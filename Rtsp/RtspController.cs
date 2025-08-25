using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdminPortalV8.Controllers;
using AdminPortalV8.Models.Epatrol;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AdminPortalV8.Rtsp
{
    [Route("api/[controller]")]
    [ApiController]
    public class RtspController : ControllerBase
    {
        private readonly EPatrol_DevContext _context;
        private readonly ILogger<CameraController> _logger;
        private readonly HttpClient _httpClient;

        public RtspController(EPatrol_DevContext context, ILogger<CameraController> logger, HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("getStreams")]
        public async Task<IActionResult> GetStreams()
        {
            //string url = "http://192.168.0.2:8083/streams"; // RTSP API endpoint
            string url = "http://192.168.1.19:8083/streams"; // RTSP API endpoint

            try
            {
                var byteArray = Encoding.ASCII.GetBytes("demo:demo");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error fetching stream data: {errorMessage}");
                    return BadRequest($"Failed to fetch stream data: {errorMessage}");
                }

                var jsonContent = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var jsonObject = JObject.Parse(jsonContent);

                // Extract "payload" from JSON
                var streamsPayload = jsonObject["payload"] as JObject;
                if (streamsPayload == null)
                {
                    return BadRequest("Stream data is missing in the response.");
                }

                var streamData = new List<object>();

                foreach (var stream in streamsPayload)
                {
                    var streamId = stream.Key; // Extract UUID key
                    var streamName = stream.Value["name"]?.ToString();
                    var channelsObj = stream.Value["channels"] as JObject;

                    // Extract the first valid RTSP URL from channels
                    string rtspUrl = null;

                    if (channelsObj != null)
                    {
                        foreach (var channel in channelsObj)
                        {
                            var channelInfo = channel.Value as JObject;
                            if (channelInfo != null && channelInfo["url"] != null)
                            {
                                rtspUrl = channelInfo["url"].ToString();
                                break; // Take the first available RTSP URL
                            }
                        }
                    }

                    // Construct response only if an RTSP URL is found
                    if (!string.IsNullOrEmpty(rtspUrl))
                    {
                        var streamInfo = new
                        {
                            StreamId = streamId, // UUID key
                            StreamName = streamName,
                            RtspUrl = rtspUrl
                        };

                        streamData.Add(streamInfo);
                    }
                }

                return Ok(streamData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred: {ex.Message}");
                return BadRequest($"Failed to parse the stream data: {ex.Message}");
            }
        }

    }
}
