using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Models.Restaurant
{
    public class RestaurantViewModel
    {
        public RestaurantDetailModel Restaurant { get; set; }
        

    }

    public class RestaurantDetailModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string Address { get; set; }
        public int Manager { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        public List<RobotModel>? Robots { get; set; }
        public RobotModel? RobotDetail { get; set; }

        public List<AnchorModel>? Anchors { get; set; }
        public AnchorModel? AnchorDetail { get; set; }
    }

    public class RobotModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Serial Number")]
        public string SerialNum { get; set; }
        public bool Active { get; set; }
        public int restaurant_id { get; set; }
        public string? robot_name { get; set; }
    }

    public class AnchorModel
    {
        public int Id { get; set; }
        [Display(Name = "Anchor Address")]
        public string Anchor_Address { get; set; }
        [Display(Name = "X - Axes")]
        public float X_Axis { get; set; }
        [Display(Name = "Y - Axes")]
        public float Y_Axis { get; set; }
        [Display(Name = "Main Anchor")]
        public bool isMainAnchor { get; set; }
        public int restaurant_id { get; set; }
    }

    public class RestaurantObj
    {
        public List<RobotModel> Robots { get; set; }
        public List<AnchorModel> Anchors { get; set; }
    }

    public class AnalyticObj
    {
        public int Analytic_RestaurantId { get; set; }
        public int Analytic_RobotId { get; set; }
        public DateTime Analytic_start { get; set; }
        public DateTime Analytic_end { get; set; }
    }
}
