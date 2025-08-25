using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminPortalV8.ViewModels;

namespace EPatrol.Services
{
    public class AutoPatrolRequestModel
    {
        public string camera { get; set; }
        public int duration { get; set; }
        public string dropbox_token { get; set; }
        public List<ModelInfo> models { get; set; }
        public string schedule_start_time { get; set; } // ISO 8601 format
        public string schedule_end_time { get; set; }   // ISO 8601 format
        public double delay_seconds { get; set; }       // Delay in seconds
    }


    public interface IAutoPtrolApiCalling
    {
        Task<string> SendAutoPatrlRequestAsync(string camera, int duration, string dropboxToken, List<ModelInfo> models,
        string scheduleStartTime,
        string scheduleEndTime,
        double delaySeconds);
    }

    public class AutoPtrolApiCalling : IAutoPtrolApiCalling, IDisposable
    {
        private readonly HttpClient _httpClient;

        public AutoPtrolApiCalling(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8001");
        }

        public async Task<string> SendAutoPatrlRequestAsync(string camera, int duration, string dropboxToken, List<ModelInfo> models,
        string scheduleStartTime,
        string scheduleEndTime,
        double delaySeconds)
        {
            // Construct the request object
            var requestObject = new AutoPatrolRequestModel
            {
                camera = camera,
                duration = duration,
                dropbox_token = dropboxToken,
                models = models,
                schedule_start_time = scheduleStartTime,
                schedule_end_time = scheduleEndTime,
                delay_seconds = delaySeconds
            };

            // Serialize to JSON
            var requestJson = JsonSerializer.Serialize(requestObject);
            //Console.WriteLine("Request JSON: " + requestJson);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");


            
            // POST to the endpoint
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.PostAsync("/press_button/images", content);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                // Read response content
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions (e.g., network error, non-2xx status)
                // Log error, rethrow or return an error message
                throw new Exception("Error calling the image API", ex);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}