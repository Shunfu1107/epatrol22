using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("anchor")]
    public class Anchor
    {
        [Key]
        public int anchor_id { get; set; }
        public int ismain_anchor { get; set; }
        public double X_Axis { get; set; }
        public double Y_Axis { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string anchor_address { get; set; }
        public int restaurant_id { get; set; }
    }
}
