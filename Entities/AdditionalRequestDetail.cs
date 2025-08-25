using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("additionalrequestdetail")]
    public class AdditionalRequestDetail
    {
        [Key]
        public int requestdetail_id { get; set; }
        public int tag_id { get; set; }
        public int table_no { get; set; }
        public int item_type { get; set; }
        public int item_qty { get; set; }
        public int additionalrequest { get; set; }
    }
}
