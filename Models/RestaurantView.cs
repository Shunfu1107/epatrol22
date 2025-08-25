using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AdminPortalV8.Models
{
    public class RestaurantView
    {
        public List<RestaurantModel> RestaurantList { get; set; }

    }

    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Anchor { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        public int Robots { get; set; }
        public string Location { get; set; }
        public string Active { get; set; }

    }

    public class UserRestaurantModel
    {
        public int pvid { get; set; }
        public int userID{ get; set; }
        public int restaurant_id { get; set; }
        public string restaurant_name { get; set; }
        public DateTime StartActiveDate { get; set; }
        public string Status { get; set; }
    }

    public class AddUserRestaurant
    {
        public int userId { get; set; }
        [Display(Name = "Restaurant")]
        public int restaurantId { get; set; }

        public IList<RestaurantModel>? RestaurantList { get; set; }
    }

    
}
