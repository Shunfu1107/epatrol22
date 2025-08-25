namespace AdminPortalV8.Models.Epatrol
{
    public class EmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }     
        public string[] CC { get; set; }
    }

    public class MailSetting
    {

        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
}