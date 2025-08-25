using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name: "IdentityAccessLevelExclusive")]
    public class IdentityAccessLevelExclusive
    {
       
        public long Pvid { get; set; }
        public long IdentityUserPvid { get; set; }
        public string PermissionKey { get; set; }
        public bool Accessible { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
