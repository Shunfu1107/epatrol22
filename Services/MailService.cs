using AdminPortalV8.Models.Epatrol;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AdminPortalV8.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailModel mailRequest);

        IFormFile CreateFromFile(string filePath);

    }

    public class MailService : IMailService
    {
        private readonly MailSetting _mailSettings;
        public MailService(IOptions<MailSetting> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(EmailModel mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();

            builder.HtmlBody = mailRequest.Body.Replace("<p>", "<p style='margin:0px'>"); ;
            //builder.TextBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public IFormFile CreateFromFile(string filePath)
        {
            try
            {
                filePath = filePath.Replace("../", "./wwwroot/");
                // Read the file data into a byte array
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Create a MemoryStream from the byte array
                MemoryStream memoryStream = new MemoryStream(fileBytes);

                // Create an IFormFile instance
                IFormFile formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, Path.GetFileName(filePath))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream" // Change to the appropriate content type
                };

                return formFile;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}