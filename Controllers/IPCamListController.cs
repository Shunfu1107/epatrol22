using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdminPortalV8.Controllers
{
    public class IPCamListController : Controller
    {
        private readonly HttpClient _httpClient;

        public IPCamListController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // The action that renders the view
        public async Task<IActionResult> Index()
        {
            // Fetch the stream data from the API
            var streamData = await GetStreamsAsync();

            // Pass the stream data to the view
            return View(streamData); // The data passed will be used in the view
        }

        // Method to call the API and get the stream data
        private async Task<List<dynamic>> GetStreamsAsync()
        {
            string url = "https://localhost:7124/api/rtsp/getStreams"; // API URL

            // Sending GET request to fetch data
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // If the response is not successful, return an empty list or handle accordingly
                return new List<dynamic>();
            }

            // Read the content and deserialize into a list of streams
            var jsonContent = await response.Content.ReadAsStringAsync();
            var streams = JsonConvert.DeserializeObject<List<dynamic>>(jsonContent);

            return streams;
        }
    }
}
