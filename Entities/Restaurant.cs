using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{

    [Table("restaurant")]
    public class Restaurant
    {
        [Key]
        public int restaurant_id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string restaurant_name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string restaurant_address { get; set; }
        public int manager { get; set; }
        public int active { get; set; }
        public DateTime startDate { get; set; }
    }
}
