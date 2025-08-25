using AdminPortalV8.Models.Epatrol;

namespace AdminPortalV8.ViewModels
{
    public class RouteEditViewModel
    {
        public int RouteId { get; set; }
        public string? RouteName { get; set; }
        public int? PatrolTypeId { get; set; }
        public List<CheckPoint>? CheckPoints { get; set; }
        public List<int> SelectedCheckpoints { get; set; } = new List<int>();
        public List<Schedule>? Schedules { get; set; }
        public List<int> SelectedSchedules { get; set; } = new List<int>();
    }
}
