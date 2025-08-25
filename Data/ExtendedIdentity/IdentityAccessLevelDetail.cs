using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name:"IdentityAccessLevelDetail")]
    public class IdentityAccessLevelDetail
    {
        
        public long Pvid { get; set; }
        public long IdentityAccessLevelPvid { get; set; }
        public long IdentityModulePvid { get; set; }
        public string PermissionKey { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
