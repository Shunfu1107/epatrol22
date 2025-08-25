using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Models.Epatrol;
using AdminPortalV8.Services;
using AdminPortalV8.ViewModels;
using EPatrol.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quartz;
using Quartz.Core;
using AdminPortalV8.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;


namespace AdminPortalV8.Controllers
{
    public class AutoPatrollingController : Controller
    {
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly EPatrol_DevContext _context;
        private readonly IFfmpegProcessService _ffmpegProcessService;
        private readonly ITelegramService _telegramService;
        private readonly ISMSService _smsService;
        private readonly IAutoPtrolApiCalling _autoPatrolRequest;
        private readonly IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IMailService _MailService;
        private readonly IEncryption _encryptionService;

        public AutoPatrollingController(UserObj usrObj, IGeneral general, EPatrol_DevContext context, IFfmpegProcessService ffmpegProcessService, ITelegramService telegramService, ISMSService smsService, IAutoPtrolApiCalling autoPtrolApiCalling, ISchedulerFactory schedulerFactory, IMailService mailService, IEncryption encryptionService)
        {
            _usrObj = usrObj;
            _general = general;
            _context = context;
            _ffmpegProcessService = ffmpegProcessService;
            _telegramService = telegramService;
            _smsService = smsService;
            _autoPatrolRequest = autoPtrolApiCalling;
            _schedulerFactory = schedulerFactory;
            _MailService = mailService;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        public IActionResult Index(int routeId)
        {
            var route = _context.Routes
                .Where(r => r.RouteId == routeId)
                .Select(r => new
                {
                    routeId = r.RouteId,
                    routeName = r.RouteName,
                    Checkpoints = r.RouteCheckPoints.Select(cp => new
                    {
                        CheckpointId = cp.CheckPointId,
                        CheckpointName = cp.CheckPoint.CheckPointName,
                        Checklists = cp.CheckPoint.CheckLists.Select(cl => new
                        {
                            ChecklistId = cl.CheckListId,
                            ChecklistName = cl.CheckListName,
                            ModelName = cl.ModelNavigation.Name,
                            ModelUrl = cl.ModelNavigation.Url
                        }).ToList(),
                        Cameras = cp.CheckPoint.Cameras.Select(cam => new
                        {
                            CameraId = cam.CameraId,
                            CameraName = cam.Name,
                            CameraIp = cam.Url
                        }).ToList()
                    }).ToList(),
                    Schedules = r.RouteSchedules.Select(rs => new
                    {
                        ScheduleId = rs.ScheduleId,  // This links to the Route_Schedule table
                        ScheduleName = rs.Schedule.ScheduleName,  // Use the actual ScheduleName from the Schedule table
                        Day = rs.Schedule.Day,
                        StartTime = rs.Schedule.StartTime,
                        EndTime = rs.Schedule.EndTime
                    }).ToList()
                })
                .FirstOrDefault();

            if (route == null)
            {
                return NotFound();
            }
            ViewBag.Route = route;
            return View(route);
        }


        [HttpGet]
        public IActionResult GetCheckpoints(int routeId)
        {
            var route = _context.Routes
                .Where(r => r.RouteId == routeId)
                .Select(r => new
                {
                    Checkpoints = r.RouteCheckPoints.Select(cp => new
                    {
                        CheckpointId = cp.CheckPointId,
                        CheckpointName = cp.CheckPoint.CheckPointName,
                        Checklists = cp.CheckPoint.CheckLists.Select(cl => new
                        {
                            ChecklistId = cl.CheckListId,
                            ChecklistName = cl.CheckListName
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (route == null)
            {
                return NotFound();
            }

            return Json(route);
        }

        //[HttpPost]
        //public IActionResult SavePatrol([FromBody] PatrolData patrolData)
        //{
        //    if (patrolData == null || !patrolData.Checklists.Any())
        //    {
        //        return BadRequest("No data to save.");
        //    }

        //    foreach (var checklist in patrolData.Checklists)
        //    {
        //        // Use the IEncryption service directly to encrypt the Link
        //        //checklist.Link = _encryptionService.Encrypt(checklist.Link);

        //        var patrol = new Patrol
        //        {
        //            RouteId = patrolData.RouteId,
        //            CheckPointName = checklist.CheckPointName,
        //            CheckListName = checklist.CheckListName,
        //            Status = checklist.Status,
        //            Note = checklist.Note,
        //            Date = DateOnly.FromDateTime(DateTime.Now),
        //            Time = TimeOnly.FromDateTime(DateTime.Now),
        //            //Link = checklist.Link
        //            VideoLink = checklist.Link
        //        };

        //        _context.Patrols.Add(patrol);
        //    }

        //    _ffmpegProcessService.StopProcess();
        //    _context.SaveChanges();

        //    return Ok("Patrol data saved successfully.");
        //}

        [HttpPost]
        public IActionResult SavePatrol([FromBody] PatrolData patrolData)
        {
            if (patrolData == null || !patrolData.Checklists.Any())
            {
                return BadRequest("No data to save.");
            }

            Console.WriteLine($"Received JSON: {JsonConvert.SerializeObject(patrolData)}");

            foreach (var checklist in patrolData.Checklists)
            {
                Console.WriteLine($"Received Video Link: {checklist.Link}");

                Console.WriteLine($"DEBUG: checklist.Link = '{checklist.Link}'");
                Console.WriteLine($"DEBUG: checklist.Link is null: {checklist.Link == null}");
                Console.WriteLine($"DEBUG: checklist.Link is empty: {string.IsNullOrEmpty(checklist.Link)}");

                byte[]? videoData = null;

                Console.WriteLine("DEBUG: Before file existence check");
                if (!string.IsNullOrEmpty(checklist.Link))
                {
                    Console.WriteLine("DEBUG: Entering file existence check");

                    try
                    {
                        if (System.IO.File.Exists(checklist.Link))
                        {
                            Console.WriteLine("DEBUG: File found!");
                            videoData = System.IO.File.ReadAllBytes(checklist.Link);
                        }
                        else
                        {
                            Console.WriteLine("DEBUG: File NOT found!");
                            return BadRequest($"File not found at {checklist.Link}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DEBUG: Exception occurred: {ex.Message}");
                        return BadRequest($"Error reading file: {ex.Message}");
                    }
                }

                TimeZoneInfo malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
                DateTime kualaLumpurTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, malaysiaTimeZone);

                var patrol = new Patrol
                {
                    RouteId = patrolData.RouteId,
                    CheckPointName = checklist.CheckPointName,
                    CheckListName = checklist.CheckListName,
                    Status = checklist.Status,
                    Note = checklist.Note,
                    Date = DateOnly.FromDateTime(kualaLumpurTime),
                    Time = TimeOnly.FromDateTime(kualaLumpurTime),
                    Link = checklist.Link ?? "",
                    VideoLink = videoData
                };

                _context.Patrols.Add(patrol);
            }

            _ffmpegProcessService.StopProcess();
            _context.SaveChanges();

            return Ok("Patrol data saved successfully.");
        }


        public IActionResult PatrolDone()
        {
            return RedirectToAction("Index", "Patrol");
        }

        [HttpPost]
        public IActionResult StopStreaming()
        {
            _ffmpegProcessService.StopProcess();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> TelegramNotifyAnomaly(string message)
        {
            //await _telegramService.SendTelegramMessageAsync("6596661148", message);
            //await _telegramService.SendTelegramGroupMessageAsync("6596661148", message);


            StringBuilder msg = new StringBuilder();

            msg.AppendLine(message + "<br/><br/>");
            msg.AppendLine("Thank you.<br/><br/>");
            msg.AppendLine("This is a system-generated email. Please do not reply to this email.");

            EmailModel mailRequest = new EmailModel();
            //mailRequest.ToEmail = "junkang_cjk@hotmail.com";
            //mailRequest.ToEmail = "bernice.ylyong@gmail.com";
            mailRequest.Subject = "Notification: Anomaly Detected";
          
            mailRequest.Body = msg.ToString();

            await _MailService.SendEmailAsync(mailRequest);

            return Ok("Message sent successfully");
        }

        [HttpPost]
        public async Task<IActionResult> SMSNotifyAnomaly(string message)
        {
            //await _smsService.SendSMSMessageAsync("6596661148", message);
            return Ok("Message sent successfully");
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleAutoPatrol([FromBody] AutoDetectionData autoDetectionData)
        {
            if (autoDetectionData == null || string.IsNullOrEmpty(autoDetectionData.Message))
            {
                return BadRequest("No message received");
            }

            var routeWithSchedules = _context.Routes
                .Where(r => r.RouteId == int.Parse(autoDetectionData.RouteId))
                .Select(r => new
                {
                    RouteId = r.RouteId,
                    RouteName = r.RouteName,
                    Schedules = r.Schedules.Select(s => new
                    {
                        ScheduleId = s.ScheduleId,
                        ScheduleName = s.ScheduleName,
                        Day = s.Day,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList()
                })
                .FirstOrDefault();

            foreach (var schedule in routeWithSchedules.Schedules)
            {
                var cronSchedule = TimeDayConverter.ConvertTimeAndDay(schedule.StartTime.ToString(), schedule.Day.ToString());

                var scheduler = await _schedulerFactory.GetScheduler();
                var job_key_name = autoDetectionData.CameraIp + schedule.ScheduleId;
                var jobKey = new JobKey(job_key_name, autoDetectionData.RouteId);
                var triggerKey = new TriggerKey(job_key_name, autoDetectionData.RouteId);

                // Check if the job already exists
                if (await scheduler.CheckExists(jobKey))
                {
                    await scheduler.DeleteJob(jobKey); // Delete the existing job
                }
                // For the withIdentiy label we should pass unique identifier for each user who create this schedule.

                IJobDetail job = JobBuilder.Create<AutoPatrolJob>()
                    .WithIdentity(jobKey)
                    // Pass parameters into the JobDataMap
                    .UsingJobData("CameraIp", autoDetectionData.CameraIp)
                    .UsingJobData("Duration", autoDetectionData.Duration)
                    .UsingJobData("DropBoxToken", autoDetectionData.DropBox_Token)
                    .UsingJobData("Models", JsonConvert.SerializeObject(autoDetectionData.Models))
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithCronSchedule(cronSchedule)
                    .Build();
                await scheduler.ScheduleJob(job, trigger);
            }

            return Ok("Start");
        }

        public async Task<IActionResult> StopScheduleAutoPatrol([FromBody] AutoDetectionData autoDetectionData)
        {
            if (autoDetectionData == null || string.IsNullOrEmpty(autoDetectionData.Message))
            {
                return BadRequest("No message received");
            }
            var scheduler = await _schedulerFactory.GetScheduler();

            var routeWithSchedules = _context.Routes
            .Where(r => r.RouteId == int.Parse(autoDetectionData.RouteId))
            .Select(r => new
            {
                RouteId = r.RouteId,
                RouteName = r.RouteName,
                Schedules = r.Schedules.Select(s => new
                {
                    ScheduleId = s.ScheduleId,
                    ScheduleName = s.ScheduleName,
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList()
            })
            .FirstOrDefault();

            foreach (var schedule in routeWithSchedules.Schedules)
            {
                var job_key_name = autoDetectionData.CameraIp + schedule.ScheduleId;
                var jobKey = new JobKey(autoDetectionData.CameraIp, autoDetectionData.RouteId);

                // Check if the job already exists
                if (await scheduler.CheckExists(jobKey))
                {
                    await scheduler.DeleteJob(jobKey); // Delete the existing job
                }

            }

            return Ok("Stop");

        }
    }
}