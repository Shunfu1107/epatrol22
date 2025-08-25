using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    public class IdentityUserProfiles
    {
        [Key]
        public long Pvid { get; set; }
        [ForeignKey("User")]
        public long IdentityUserPvid { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }

    }
}
