using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("additionalrequest")]
    public class AdditionalRequest
    {
        [Key]
        public int additionalrequest_id { get; set; }
        public int branch_additionalrequest_id { get; set; }
        public DateTime delivery_starttime { get; set; }
        public DateTime delivery_endtime { get; set; }
        public int robot_id { get; set; }
        public int restaurant_id { get; set; }
    }
}
