using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Entities
{
    [Table("taginfo")]
    public class TagInfo
    {
        [Key]
        public int tag_id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string tag_address { get; set; }
        public int restaurant_id { get; set; }
    }
}
