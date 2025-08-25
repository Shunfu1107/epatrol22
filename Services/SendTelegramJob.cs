using AdminPortalV8.Services;
using Quartz;
using System;
using System.Threading.Tasks;

public class SendTelegramJob : IJob
{
    private readonly ITelegramService _telegramService;

    public SendTelegramJob(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var message = context.MergedJobDataMap.GetString("message");
        var notificationMethod = context.MergedJobDataMap.GetString("NotificationMethod");

        if (string.IsNullOrEmpty(notificationMethod) || notificationMethod == "Telegram")
        {
            await _telegramService.SendTelegramGroupMessageAsync("6596661148", "Telegram");
            Console.WriteLine($"[Telegram] Sent: {message}");
        }
    }
}
