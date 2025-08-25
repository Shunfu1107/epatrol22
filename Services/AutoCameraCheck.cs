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
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Models;

public class AutoCameraCheck : IJob
{
    private readonly IFfmpegProcessService _ffmpegProcessService;
    private readonly IImageQualityAnalyzer _imageQualityAnalyzer;
    private readonly IJobResultService _jobResultService;
    private readonly EPatrol_DevContext _context;
    private readonly ITelegramService _telegramService;
    private readonly ISMSService _smsService;
    private readonly IMessageService _messageService;

    public AutoCameraCheck(IFfmpegProcessService ffmpegProcessService, IImageQualityAnalyzer imageQualityAnalyzer, IJobResultService jobResultService, EPatrol_DevContext context, ITelegramService telegramService, ISMSService sMSService, IMessageService messageService)
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
        var notificationMethod = context.MergedJobDataMap.GetString("notificationMethod");

        var cameraName = _context.Cameras
        .Where(c => c.CameraId == int.Parse(cameraId))
        .Select(c => c.Name)
        .FirstOrDefault();

        // extract frame
        var frame_dir = _ffmpegProcessService.ExtractFrame(cameraUrl, cameraId);
        var problem_flag = false;
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
                        statusMessage = "Blur Frame Detected";
                        _jobResultService.AddResult(cameraId, cameraName, "Blur or Dark Frame Detected");
                        problem_flag = true;
                        break;
                    }
                    else if (dark == true)
                    {
                        Console.WriteLine("TOO DARK");
                        _jobResultService.AddResult(cameraId, cameraName, "Blur or Dark Frame Detected");
                        statusMessage = "Dark Frame Detected";
                        problem_flag = true;
                        break;
                    };
                }
            }
            if (problem_flag == false)
            {
                _jobResultService.AddResult(cameraId, cameraName, "Camera is working correctly.");
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
            statusMessage = "Camera is not connected.";

        }

        var status = DetermineStatus(statusMessage);

        //_context.CameraStatuses.Add(new CameraStatus
        //{
        //    CameraId = int.Parse(cameraId),
        //    //CameraName = cameraName,
        //    Note = statusMessage,
        //    Status = status,
        //    StatusDate = DateTime.Now
        //});

        //_context.CameraStatuses.Add(new CameraStatus
        //{
        //    CameraId = int.Parse(cameraId),
        //    Note = statusMessage,
        //    Status = status,
        //    StatusDate = DateTime.Now
        //});

        try
        {
            // Save status to CameraStatusNews table
            _context.CameraStatusNews.Add(new CameraStatusNew
            {
                CameraId = int.Parse(cameraId),
                Note = statusMessage,
                Status = status,
                StatusDate = DateTime.Now,
                NotificationMethod = notificationMethod ?? "unknown"
            });

            int camId = int.Parse(cameraId);

            // 🔧 Update IsActive in Cameras table
            var cameraObj = await _context.Cameras.FirstOrDefaultAsync(c => c.CameraId == camId);
            if (cameraObj != null)
            {
                // Set isActive = 1 for Working, 0 for anything else
                bool isOnline = status.Equals("Working", StringComparison.OrdinalIgnoreCase);
                cameraObj.IsActive = isOnline;

                Console.WriteLine($"[CameraId: {camId}] Status = '{status}' → IsActive set to {(isOnline ? 1 : 0)}");
            }
            else
            {
                Console.WriteLine($"CameraId {camId} not found in database. Skipping IsActive update.");
            }

            if (status != "Working")
            {
                var location = cameraObj?.Location?.Name + " " + cameraObj?.Location?.Level;

                if (!_context.Notifications.Any(n =>
                    n.Device == cameraName &&
                    n.Type == "Camera Status" &&
                    n.Timestamp == DateTime.Today &&
                    n.Note == statusMessage))
                {
                    _context.Notifications.Add(new Notification
                    {
                        Device = cameraName,
                        Type = "Camera Status",
                        Location = location ?? "-",
                        Timestamp = DateTime.Now,
                        Note = statusMessage
                    });
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Data successfully inserted into CameraStatus and Camera updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to database: {ex.Message}");
        }

        //try
        //{
        //    _context.SaveChanges();
        //    Console.WriteLine("Data successfully inserted into CameraStatus.");
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Error saving to database: {ex.Message}");
        //}

        await NotifyUsers(cameraId, cameraName, statusMessage, notificationMethod);

        Console.WriteLine("@@@ Schedule Done @@@");

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
        else if (note.Contains("Blur Frame Detected", StringComparison.OrdinalIgnoreCase))
        {
            return "Blurry";
        }
        else if (note.Contains("Dark Frame Detected", StringComparison.OrdinalIgnoreCase))
        {
            return "Dark";
        }
        else
        {
            return "Unknown";
        }
    }

    private async Task NotifyUsers(string cameraId, string cameraName, string statusMessage, string notificationMethod)
    {
        var message = $"Camera: {cameraName} (ID: {cameraId})\nStatus: {statusMessage}";

        if (notificationMethod == "Telegram")
        {
            //await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);

            // Send via Telegram
            //await _telegramService.SendTelegramMessageAsync("6596661148", message);
            Console.WriteLine("Send via telegram.....");
            Console.WriteLine(message);
        }
        else if (notificationMethod == "SMS")
        {
            // Send via SMS
            //await _smsService.SendSMSMessageAsync("6593684088", message);
            Console.WriteLine("Send via sms.....");
            Console.WriteLine(message);
        }
        else if (notificationMethod == "WhatsApp")
        {
            // Send via WhatsApp
            //await _messageService.SendGroupMessageAsync("6596661148", message);
            Console.WriteLine("Send via WhatsApp.....");
            Console.WriteLine(message);
        }
        else if (notificationMethod == "Both")
        {
            //await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);

            // Send via Telegram
            //await _telegramService.SendTelegramMessageAsync("6596661148", message);
            Console.WriteLine("Send via telegram.....");
            //await _smsService.SendSMSMessageAsync("6593684088", message);
            Console.WriteLine("Send via sms.....");
            //await _messageService.SendGroupMessageAsync("6596661148", message);
            Console.WriteLine("Send via WhatsApp.....");
            Console.WriteLine(message);
        }
        Console.WriteLine("Notification sent to users.");
    }
}


