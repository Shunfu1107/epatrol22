using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Quartz;
using System.Threading.Tasks;
using EPatrol.Services;
using Newtonsoft.Json;
using AdminPortalV8.ViewModels;
using AdminPortalV8.Services;
using iText.IO.Font.Constants;
using AdminPortalV8.Models.Epatrol;
using ZXing.Client.Result;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Models;

public class AutoCamHealthCheckJob : IJob
{
    private readonly IFfmpegProcessService _ffmpegProcessService;
    private readonly IImageQualityAnalyzer _imageQualityAnalyzer;
    private readonly IJobResultService _jobResultService;
    private readonly EPatrol_DevContext _context;
    private readonly ITelegramService _telegramService;
    private readonly ISMSService _smsService;
    private readonly IMessageService _messageService;

    public AutoCamHealthCheckJob(IFfmpegProcessService ffmpegProcessService, IImageQualityAnalyzer imageQualityAnalyzer, IJobResultService jobResultService, EPatrol_DevContext context, ITelegramService telegramService, ISMSService sMSService, IMessageService messageService)
    {
        _ffmpegProcessService = ffmpegProcessService;
        _imageQualityAnalyzer = imageQualityAnalyzer;
        _jobResultService = jobResultService;
        _context = context;
        _telegramService = telegramService;
        _smsService = sMSService;
        _messageService = messageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("@@@ Schedule Start @@@");
        var cameraUrl = context.MergedJobDataMap.GetString("CameraUrl");
        var cameraId = context.MergedJobDataMap.GetString("CameraId");
        var notificationMethod = context.MergedJobDataMap.GetString("NotificationMethod") ?? "None"; // Get method from job

        var cameraName = _context.Cameras
        .Where(c => c.CameraId == int.Parse(cameraId))
        .Select(c => c.Name)
        .FirstOrDefault();

        // extract frame
        var frame_dir = _ffmpegProcessService.ExtractFrame(cameraUrl, cameraId);
        var problem_flag = false;
        var status = "";
        var statusMessage = "";

        if (frame_dir != null)
        {
            // Retrieve all file paths under frame_dir
            var files = Directory.GetFiles(frame_dir);
            // check the darkness of frame
            foreach (var filePath in files)
            {
                Console.WriteLine(filePath);
                var blur = _imageQualityAnalyzer.GetAverageBrightness(filePath);
                var dark = _imageQualityAnalyzer.IsBlurry(filePath);
                // For example, do some "darkness check" on the frame
                if (blur || dark)
                {
                    if (blur == true)
                    {
                        Console.WriteLine("TOO BLUR");
                        status = "Blur";
                        statusMessage = "Blur Frame Detected";
                        _jobResultService.AddResult(cameraId, cameraName, "Blur or Dark Frame Detected");
                        problem_flag = true;
                        break;
                    }
                    else if (dark == true)
                    {
                        Console.WriteLine("TOO DARK");
                        _jobResultService.AddResult(cameraId, cameraName, "Blur or Dark Frame Detected");
                        status = "Dark";
                        statusMessage = "Dark Frame Detected";
                        problem_flag = true;
                        break;
                    };
                }
            }
            if (problem_flag == false)
            {
                _jobResultService.AddResult(cameraId, cameraName, "Camera is working correctly.");
                status = "Working";
                statusMessage = "Camera is working correctly.";
            }
            Console.WriteLine("@@@ Schedule Done @@@");
            if (Directory.Exists(frame_dir))
            {
                Directory.Delete(frame_dir, recursive: true);
            }

            // tell user that camera is working

        }
        else
        {
            if (Directory.Exists(frame_dir))
            {
                Directory.Delete(frame_dir, recursive: true);
            }
            _jobResultService.AddResult(cameraId, cameraName, "Camera is not connected.");
            // tell user that camera connection have some problem
            Console.WriteLine("Check the camera connection");
            status = "Not Working";
            statusMessage = "Camera is not connected.";

        }

        var blockName = context.MergedJobDataMap.GetString("BlockName");
        var level = context.MergedJobDataMap.GetString("Level");
        var timestamp = DateTime.Now;

        _context.CameraHealthResults.Add(new CameraHealthResult
        {
            CameraId = int.Parse(cameraId),
            CameraName = cameraName,
            Status = status,
            Result = statusMessage,
            Timestamp = timestamp,
            BlockName = blockName,
            Level = level
        });

        if (status != "Working")
        {
            if (!_context.Notifications.Any(n =>
                n.Device == cameraName &&
                n.Type == "Camera Health" &&
                n.Timestamp == timestamp.Date &&
                n.Note == statusMessage))
            {
                _context.Notifications.Add(new Notification
                {
                    Device = cameraName,
                    Type = "Camera Health",
                    Location = $"{blockName} {level}",
                    Timestamp = timestamp,
                    Note = statusMessage
                });
            }
        }

        _context.SaveChanges();

        //await NotifyUsers(cameraId, cameraName, statusMessage);

        Console.WriteLine("@@@ Schedule Done @@@");

        Console.WriteLine($"Notification method found: {notificationMethod}");

        if (notificationMethod != "None")
        {
            await SendNotification(cameraId, cameraName, statusMessage, notificationMethod);
        }

    }

    private async Task SendNotification(string cameraId, string cameraName, string statusMessage, string method)
    {
        var message = $"Camera: {cameraName} (ID: {cameraId})\nStatus: {statusMessage}";

        if (method == "Telegram")
        {
            Console.WriteLine("Sending Telegram Notification...");
            //await _telegramService.SendTelegramMessageAsync("6596661148", message);
            //await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);
            Console.WriteLine(message);
        }
        else if (method == "SMS")
        {
            Console.WriteLine("Sending SMS Notification...");
            //await _smsService.SendSMSMessageAsync("6596661148", message);
            Console.WriteLine(message);
        }
        else if (method == "WhatsApp")
        {
            Console.WriteLine("Sending WhatsApp Notification...");
            //await _messageService.SendGroupMessageAsync("6596661148", message);
            Console.WriteLine(message);
        }
        else if (method == "Both")
        {
            Console.WriteLine("Sending Both Notifications...");
            //await _telegramService.SendTelegramMessageAsync("6596661148", message);
            //await _smsService.SendSMSMessageAsync("6593684088", message);
        }
    }

    private string DetermineStatus(string note)
    {
        if (note.Contains("not connected", StringComparison.OrdinalIgnoreCase))
        {
            return "Not Working";
        }
        else if (note.Contains("working correctly", StringComparison.OrdinalIgnoreCase))
        {
            return "Working";
        }
        else if (note.Contains("blurry", StringComparison.OrdinalIgnoreCase))
        {
            return "Blurry";
        }
        else if (note.Contains("dark", StringComparison.OrdinalIgnoreCase))
        {
            return "Dark";
        }
        else
        {
            return "Unknown";
        }
    }

    private async Task NotifyUsers(string cameraId, string cameraName, string statusMessage)
    {
        var message = $"Camera: {cameraName} (ID: {cameraId})\nStatus: {statusMessage}";

        //await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);

        // Send via Telegram
        //await _telegramService.SendTelegramMessageAsync("6596661148", message);

        // Send via SMS
        //await _smsService.SendSMSMessageAsync("6596661148", message);

        Console.WriteLine("Notification sent to users.");
    }
}