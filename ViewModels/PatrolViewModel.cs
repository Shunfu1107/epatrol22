using AdminPortalV8.Models.Epatrol;
using Org.BouncyCastle.Crypto.Parameters;

namespace AdminPortalV8.ViewModels
{
    public class PatrolData
    {
        public int RouteId { get; set; }
        public List<ChecklistData> Checklists { get; set; }

        //public int GuardId { get; set; }

        public string GuardName { get; set; }

    }

    public class ChecklistData
    {
        public int CheckPointId { get; set; }
        public string CheckPointName { get; set; }
        public string CheckListName { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string Link { get; set; }
    }
    public class AutoDetectionData
    {
        public string Message { get; set; }
        public string DropBox_Token { get; set; }
        public string Duration { get; set; }
        public string RouteId { get; set; }
        public string RouteName { get; set; }
        public string CameraIp { get; set; }
        public List<ModelInfo> Models { get; set; }
        public string ScheduleStartTime { get; set; } // ISO 8601 format
        public string ScheduleEndTime { get; set; }   // ISO 8601 format
        public double DelaySeconds { get; set; }      // Delay in seconds
    }

    public class ModelInfo
    {
        public string name { get; set; }
        public string url { get; set; }
    }
}
