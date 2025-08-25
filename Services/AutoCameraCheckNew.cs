//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//using Quartz;
//using System.Threading.Tasks;
//using EPatrol.Services;
//using Newtonsoft.Json;
//using AdminPortalV8.ViewModels;
//using AdminPortalV8.Services;
//using iText.IO.Font.Constants;
//using AdminPortalV8.Models.Epatrol;
//using ZXing.Client.Result;
//using Microsoft.AspNetCore.Mvc;

//public class AutoCameraCheckNew : IJob
//{
//    private readonly IFfmpegProcessService _ffmpegProcessService;
//    private readonly IImageQualityAnalyzer _imageQualityAnalyzer;
//    private readonly IJobResultService _jobResultService;
//    private readonly EPatrol_DevContext _context;
//    private readonly ITelegramService _telegramService;
//    private readonly ISMSService _smsService;

//    public AutoCameraCheckNew(IFfmpegProcessService ffmpegProcessService, IImageQualityAnalyzer imageQualityAnalyzer, IJobResultService jobResultService, EPatrol_DevContext context, ITelegramService telegramService, ISMSService sMSService)
//    {
//        _ffmpegProcessService = ffmpegProcessService;
//        _imageQualityAnalyzer = imageQualityAnalyzer;
//        _jobResultService = jobResultService;
//        _context = context;
//        _telegramService = telegramService;
//        _smsService = sMSService;
//    }

//    public async Task Execute(IJobExecutionContext context)
//    {
//        Console.WriteLine("@@@ Schedule Start @@@");
//        var RTSPUrl = context.MergedJobDataMap.GetString("RTSPUrl");
//        var locationCameraId = context.MergedJobDataMap.GetString("locationCameraId");

//        var locationCameraName = _context.LocationCameras
//        .Where(c => c.LocationCameraId == int.Parse(locationCameraId))
//        .Select(c => c.Name)
//        .FirstOrDefault();

//        // extract frame
//        var frame_dir = _ffmpegProcessService.ExtractFrame(RTSPUrl, locationCameraId);
//        var problem_flag = false;
//        var statusMessage = "";

//        if (frame_dir != null)
//        {
//            // Retrieve all file paths under frame_dir
//            var files = Directory.GetFiles(frame_dir);
//            // check the darkness of frame
//            foreach (var filePath in files)
//            {
//                Console.WriteLine(filePath);
//                var blur = _imageQualityAnalyzer.GetAverageBrightness(filePath);
//                var dark = _imageQualityAnalyzer.IsBlurry(filePath);
//                // For example, do some "darkness check" on the frame
//                if (blur || dark)
//                {
//                    if (blur == true)
//                    {
//                        Console.WriteLine("TOO BLUR");
//                        statusMessage = "Blur Frame Detected";
//                        _jobResultService.AddResult(locationCameraId, locationCameraName, "Blur or Dark Frame Detected");
//                        problem_flag = true;
//                        break;
//                    }
//                    else if (dark == true)
//                    {
//                        Console.WriteLine("TOO DARK");
//                        _jobResultService.AddResult(locationCameraId, locationCameraName, "Blur or Dark Frame Detected");
//                        statusMessage = "Dark Frame Detected";
//                        problem_flag = true;
//                        break;
//                    };
//                }
//            }
//            if (problem_flag == false)
//            {
//                _jobResultService.AddResult(locationCameraId, locationCameraName, "Camera is working correctly.");
//                statusMessage = "Camera is working correctly.";
//            }
//            Console.WriteLine("@@@ Schedule Done @@@");
//            if (Directory.Exists(frame_dir))
//            {
//                Directory.Delete(frame_dir, recursive: true);
//            }

//            // tell user that camera is working

//        }
//        else
//        {
//            if (Directory.Exists(frame_dir))
//            {
//                Directory.Delete(frame_dir, recursive: true);
//            }
//            _jobResultService.AddResult(locationCameraId, locationCameraName, "Camera is not connected.");
//            // tell user that camera connection have some problem
//            Console.WriteLine("Check the camera connection");
//            statusMessage = "Camera is not connected.";

//        }

//        var status = DetermineStatus(statusMessage);

//        try
//        {
//            _context.CameraStatuses.Add(new CameraStatus
//            {
//                CameraId = int.Parse(locationCameraId),
//                Note = statusMessage,
//                Status = status,
//                StatusDate = DateTime.Now
//            });

//            _context.SaveChanges();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error saving to database: {ex.Message}");
//        }

//        await NotifyUsers(locationCameraId, locationCameraName, statusMessage);

//        Console.WriteLine("@@@ Schedule Done @@@");

//    }

//    private string DetermineStatus(string note)
//    {
//        if (note.Contains("not connected", StringComparison.OrdinalIgnoreCase))
//        {
//            return "Not Working";
//        }
//        else if (note.Contains("working correctly", StringComparison.OrdinalIgnoreCase))
//        {
//            return "Working";
//        }
//        else if (note.Contains("blurry", StringComparison.OrdinalIgnoreCase))
//        {
//            return "Blurry";
//        }
//        else if (note.Contains("dark", StringComparison.OrdinalIgnoreCase))
//        {
//            return "Dark";
//        }
//        else
//        {
//            return "Unknown";
//        }
//    }

//    private async Task NotifyUsers(string cameraId, string cameraName, string statusMessage)
//    {
//        var message = $"Camera: {cameraName} (ID: {cameraId})\nStatus: {statusMessage}";

//        await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);

//        // Send via Telegram
//        //await _telegramService.SendTelegramMessageAsync("60142713849", message);

//        // Send via SMS
//        //await _smsService.SendSMSMessageAsync("60142713849", message);

//        Console.WriteLine("Notification sent to users.");
//    }
//}


