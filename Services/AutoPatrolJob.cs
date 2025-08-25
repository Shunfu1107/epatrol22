using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using EPatrol.Services;
using Newtonsoft.Json;
using AdminPortalV8.ViewModels;

public class AutoPatrolJob : IJob
{
    private readonly IAutoPtrolApiCalling _autoPatrolRequest;

    public AutoPatrolJob(IAutoPtrolApiCalling autoPtrolApiCalling)
    {
        _autoPatrolRequest = autoPtrolApiCalling;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Extract parameters from JobDataMap
        var cameraIp = context.MergedJobDataMap.GetString("CameraIp");
        var durationStr = context.MergedJobDataMap.GetString("Duration");
        var dropBoxToken = context.MergedJobDataMap.GetString("DropBoxToken");
        var jsonModels = context.MergedJobDataMap.GetString("Models");
        var scheduleStartTime = context.MergedJobDataMap.GetString("ScheduleStartTime");
        var scheduleEndTime = context.MergedJobDataMap.GetString("ScheduleEndTime");
        var delaySecondsStr = context.MergedJobDataMap.GetString("DelaySeconds");

        // Validate required parameters
        if (string.IsNullOrEmpty(cameraIp) || string.IsNullOrEmpty(durationStr) || string.IsNullOrEmpty(dropBoxToken) || string.IsNullOrEmpty(jsonModels))
        {
            throw new JobExecutionException("Missing required parameters in JobDataMap (CameraIp, Duration, DropBoxToken, or Models).");
        }

        if (string.IsNullOrEmpty(scheduleStartTime) || string.IsNullOrEmpty(scheduleEndTime) || string.IsNullOrEmpty(delaySecondsStr))
        {
            throw new JobExecutionException("Missing required scheduling parameters in JobDataMap (ScheduleStartTime, ScheduleEndTime, or DelaySeconds).");
        }

        // Deserialize the models
        var models = JsonConvert.DeserializeObject<List<ModelInfo>>(jsonModels);
        if (models == null || !models.Any())
        {
            throw new JobExecutionException("Failed to deserialize Models or no models provided.");
        }

        // Parse the duration and delaySeconds
        if (!int.TryParse(durationStr, out int duration))
        {
            throw new JobExecutionException("Invalid Duration format in JobDataMap.");
        }

        if (!double.TryParse(delaySecondsStr, out double delaySeconds))
        {
            throw new JobExecutionException("Invalid DelaySeconds format in JobDataMap.");
        }

        // Call the updated SendAutoPatrlRequestAsync method with all required parameters
        await _autoPatrolRequest.SendAutoPatrlRequestAsync(
            cameraIp,
            duration,
            dropBoxToken,
            models,
            scheduleStartTime,
            scheduleEndTime,
            delaySeconds
        );
    }
}