using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("order")]
    public class Order
    {
        [Key]
        public int order_id { get; set; }
        public int branch_order_id { get; set; }
        public int tag_id { get; set; }
        public int table_no { get; set; }
        public DateTime delivery_starttime { get; set; }
        public DateTime delivery_endtime { get; set; }
        public int robot_id { get; set; }
        public int restaurant_id { get; set; }
    }
}
