using AdminPortalV8.Models.Epatrol;
using System.ComponentModel;

namespace AdminPortalV8.ViewModels
{
    public class PatrollingViewModel
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string MapBase64 { get; set; }
        public List<CheckpointViewModel> Checkpoints { get; set; }
    }

    public class CheckpointViewModel
    {
        public int CheckpointId { get; set; }
        public string CheckpointName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public string Level { get; set; }
        public List<ChecklistItemViewModel> Checklists { get; set; }
    }
    public class ChecklistItemViewModel
    {
        public int ChecklistId { get; set; }
        public string ChecklistName { get; set; }
    }
}
