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
    public interface ITelegramService
    {
        Task SendTelegramMessageAsync(string number, string message);
        Task SendTelegramGroupMessageAsync(string number, string message);
    }

    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;

        public TelegramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static string INSTANCE_ID = "34";
        private static string CLIENT_ID = "yautong.cheng@mquestsys.com";
        private static string CLIENT_SECRET = "f80f5d6b82f541d3b4e5bb1a4165cada";
        private static string API_URL = "https://enterprise.whatsmate.net/v3/telegram/single/text/message/" + INSTANCE_ID;
        private static string API_URL_GROUP = "https://enterprise.whatsmate.net/v3/telegram/group/text/message/" + INSTANCE_ID;

        public async Task SendTelegramMessageAsync(string number, string message)
        {
            var payload = new
            {
                number = number,
                message = message
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-ID", CLIENT_ID);
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-SECRET", CLIENT_SECRET);

            var response = await _httpClient.PostAsync(API_URL, jsonContent);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send message: {responseBody}");
            }
        }

        public async Task SendTelegramGroupMessageAsync(string number, string message)
        {

            var payload = new
            {
                group_name = "MQuest Malaysia",
                group_admin = number,
                message = message
            };


            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-ID", CLIENT_ID);
            _httpClient.DefaultRequestHeaders.Add("X-WM-CLIENT-SECRET", CLIENT_SECRET);

            var response = await _httpClient.PostAsync(API_URL_GROUP, jsonContent);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send message: {responseBody}");
            }
        }
    }
}