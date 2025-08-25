using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("tablecoordinate")]
    public class TableCoordinate
    {
        [Key]
        public int tablecoordinate_id { get; set; }
        public double point_1x { get; set; }
        public double point_1y { get; set; }
        public double point_2x { get; set; }
        public double point_2y { get; set; }
        public double point_3x { get; set; }
        public double point_3y { get; set; }
        public double point_4x { get; set; }
        public double point_4y { get; set; }
        public int table_id { get; set; }
        public int restaurant_id { get; set; }

    }

}

