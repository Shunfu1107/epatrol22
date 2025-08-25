using AdminPortalV8.Models.Epatrol;

namespace AdminPortalV8.ViewModels
{
    public class CheckPointViewModel
    {
        public List<CheckPoint> Checkpoints { get; set; }
        public List<CheckList> Checklists { get; set; }
        public List<Camera> Cameras { get; set; }
    }
}
