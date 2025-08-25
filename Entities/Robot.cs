using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("robot")]
    public class Robot
    {
        [Key]
        public int robot_id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string robot_serialnum { get; set; }
        public DateTime created_datetime { get; set; }
        public int active { get; set; }
        public int restaurant_id { get; set; }
        public string? robot_name { get; set; }

    }
}
