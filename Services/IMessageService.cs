using AdminPortalV8.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace AdminPortalV8.Services
{
    public interface IMessageService
    {
        Task SendGroupMessageAsync(string adminNumber, string message);
    }

    public class WhatsAppService : IMessageService
    {
        private readonly HttpClient _httpClient;

        public WhatsAppService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static string INSTANCE_ID = "28";
        private static string CLIENT_ID = "yautong.cheng@blossomseeds.sg";
        private static string CLIENT_SECRET = "e60c646beebc4ab0959345a1ebd4daa9";
        private static string API_URL_GROUP = "http://api.whatsmate.net/v3/whatsapp/group/text/message/" + INSTANCE_ID;

        public async Task SendGroupMessageAsync(string adminNumber, string message)
        {
            var payload = new
            {
                group_name = "E-Patrol Alarm",
                group_admin = adminNumber,
                message = message
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-ID", CLIENT_ID);
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-SECRET", CLIENT_SECRET);

            var response = await _httpClient.PostAsync(API_URL_GROUP, jsonContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[WhatsApp API Response]: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send WhatsApp group message: {responseBody}");
            }
        }
    }
}