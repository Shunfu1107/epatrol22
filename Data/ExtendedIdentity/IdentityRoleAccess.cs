using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name: "IdentityRoleAccess")]
    public class IdentityRoleAccess
    {
        
        public long Pvid { get; set; }
        public long IdentityRolePvid { get; set; }
        public long IdentityModulePvid { get; set; }
        public long IdentityAccessLevelPvid { get; set; }
        public long GrantedByIdentityUserPvid { get; set; }
        public DateTime GrantedDate { get; set; }

        public int? CID { get; set; }

        public string? referID { get; set; }
    }
}
