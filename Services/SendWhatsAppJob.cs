using AdminPortalV8.Services;
using Quartz;
using System;
using System.Threading.Tasks;

public class SendWhatsAppJob : IJob
{
    private readonly IMessageService _messageService;

    public SendWhatsAppJob(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var message = context.MergedJobDataMap.GetString("message");
        var notificationMethod = context.MergedJobDataMap.GetString("NotificationMethod");

        if (string.IsNullOrEmpty(notificationMethod) || notificationMethod == "WhatsApp")
        {
            await _messageService.SendGroupMessageAsync("6596661148", "WhatsApp");
            Console.WriteLine($"[WhatsApp] Sent: {message}");
        }
    }
}
