using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace AdminPortalV8.Controllers
{
    public class VerifyOtpController : Controller
    {
        private readonly IAuth _authService;
        private readonly ILogger<VerifyOtpController> _logger;
        private readonly ITelegramService _telegramService;

        public VerifyOtpController(IAuth authService, ILogger<VerifyOtpController> logger, ITelegramService telegramService)
        {
            _authService = authService;
            _logger = logger;
            _telegramService = telegramService;
        }

        [BindProperty]
        public string Sent_Otp { get; set; }

        // This method is used to show the OTP input form
        public IActionResult Index(string phoneNumber)
        {
            // Send the OTP if it hasn't been sent already
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SentOtp")))
            {
                return RedirectToAction("SendOtp", new { phoneNumber });
            }

            return View();
        }

        // Method to generate OTP
        public string GenerateOtp(int length = 6)
        {
            // Define the character set for the OTP
            const string digits = "0123456789";
            var random = new Random();

            Sent_Otp = new string(Enumerable.Range(0, length).Select(_ => digits[random.Next(digits.Length)]).ToArray());

            return Sent_Otp;
        }

        // This method sends the OTP via SMS
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {
            var otp = GenerateOtp(); // Generate OTP
            HttpContext.Session.SetString("SentOtp", otp);  // Store OTP in session
            HttpContext.Session.SetString("PhoneNumber", phoneNumber);  // Store Phone Number in session

            // Call external SMS API to send OTP
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { PhoneNumber = phoneNumber, Message = otp }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://202.73.50.123:5678/api/APITesting/SendSMS", content);
            await _telegramService.SendTelegramMessageAsync(phoneNumber, otp);

            if (response.IsSuccessStatusCode)
            {
                // Store OTP expiration time (e.g., 1 minutes)
                var expirationTime = DateTime.Now.AddMinutes(1);
                HttpContext.Session.SetString("OtpExpiration", expirationTime.ToString("o"));

                return RedirectToAction("Index", "VerifyOtp");
            }

            TempData["Message"] = "Failed to send OTP. Please try again.";
            return View("Index");
        }

        // This method handles OTP verification
        [HttpPost]
        public IActionResult CheckOtp(string Otp)
        {
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");

            // Retrieve the stored OTP and expiration time from the session
            var storedOtp = HttpContext.Session.GetString("SentOtp");
            var expirationTime = DateTime.TryParse(HttpContext.Session.GetString("OtpExpiration"), out var expiration) ? expiration : DateTime.MinValue;

            // Check if the OTP is expired or not
            if (string.IsNullOrEmpty(storedOtp) || DateTime.Now > expirationTime)
            {
                TempData["Message"] = "OTP expired. Please request a new OTP.";
                return RedirectToAction("Index");
            }

            // Validate the entered OTP
            if (storedOtp == Otp)
            {
                // OTP is valid, redirect to the dashboard
                HttpContext.Session.Remove("SentOtp");  // Clear OTP after validation
                HttpContext.Session.Remove("OtpExpiration");  // Clear expiration time

                // Redirect to the dashboard or home page after successful OTP verification
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // OTP is invalid
                TempData["Message"] = "Invalid OTP. Please try again.";
                return RedirectToAction("Index", "VerifyOtp");
            }
        }

        // This method re-sends the OTP via SMS
        public async Task<IActionResult> ResendOtp()
        {
            try
            {
                var newOtp = GenerateOtp();
                HttpContext.Session.SetString("SentOtp", newOtp);
                var phoneNumber = HttpContext.Session.GetString("PhoneNumber");

                // Log the payload for debugging
                var payload = JsonConvert.SerializeObject(new { PhoneNumber = phoneNumber, Message = newOtp });
                _logger.LogInformation("Sending payload to SMS API: {Payload}", payload);

                var client = new HttpClient();
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://202.73.50.123:5678/api/APITesting/SendSMS", content);
                await _telegramService.SendTelegramMessageAsync(phoneNumber, newOtp);

                if (response.IsSuccessStatusCode)
                {
                    var expirationTime = DateTime.Now.AddMinutes(1);
                    HttpContext.Session.SetString("OtpExpiration", expirationTime.ToString("o"));

                    TempData["Message-Success"] = "A new OTP has been sent successfully.";
                    return RedirectToAction("Index");
                }

                // Log response content on failure
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to resend OTP. Status code: {StatusCode}, Response: {ResponseContent}",
                    response.StatusCode, responseContent);

                TempData["Message"] = "Failed to resend OTP. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resending OTP.");
                TempData["Message"] = "An unexpected error occurred. Please try again.";
            }
            return RedirectToAction("Index");
        }
    }
}
