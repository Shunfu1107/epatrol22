using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using EPatrol.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace AdminPortalV8.Controllers
{
    public class SendMessageController : Controller
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ITelegramService _telegramService;
        private readonly IMessageService _messageService;

        public SendMessageController(ISchedulerFactory schedulerFactory, ITelegramService telegramService, IMessageService messageService)
        {
            _schedulerFactory = schedulerFactory;
            _telegramService = telegramService;
            _messageService = messageService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendTelegramMessage(string Time, string NotificationMethod, string Message)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            if (!DateTime.TryParse(Time, out DateTime scheduleTime))
            {
                return BadRequest("Invalid time format.");
            }

            var jobKey = new JobKey($"TelegramJob-{scheduleTime:HHmmss}");
            var triggerKey = new TriggerKey($"Trigger-{scheduleTime:HHmmss}");

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            var job = JobBuilder.Create<SendTelegramJob>()
                .WithIdentity(jobKey)
                .UsingJobData("message", Message)
                .UsingJobData("NotificationMethod", NotificationMethod)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(scheduleTime) 
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendWhatsAppMessage(string Time, string NotificationMethod, string Message)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            if (!DateTime.TryParse(Time, out DateTime scheduleTime))
            {
                return BadRequest("Invalid time format.");
            }

            var jobKey = new JobKey($"WhatsAppJob-{scheduleTime:HHmmss}");
            var triggerKey = new TriggerKey($"Trigger-{scheduleTime:HHmmss}");

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            var job = JobBuilder.Create<SendWhatsAppJob>()
                .WithIdentity(jobKey)
                .UsingJobData("message", Message)
                .UsingJobData("NotificationMethod", NotificationMethod)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(scheduleTime) 
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return View("Index");
        }
    }
}
