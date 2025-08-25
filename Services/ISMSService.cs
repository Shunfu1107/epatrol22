using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPortalV8.Services
{
    public interface ISMSService
    {
        Task SendSMSMessageAsync(string number, string message);
    }

    public class SMSService : ISMSService
    {
        private readonly HttpClient _httpClient;

        public SMSService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendSMSMessageAsync(string number, string message)
        {
            // Define the API URL
            var apiUrl = "http://202.73.50.123:5678/api/APITesting/SendSMS";

            // Create the payload
            var payload = new
            {
                PhoneNumber = number,
                Message = message
            };

            // Serialize the payload to JSON
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            // Ensure the response is successful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to send SMS. Status code: {response.StatusCode}. Response: {errorContent}");
            }
        }
    }
}
