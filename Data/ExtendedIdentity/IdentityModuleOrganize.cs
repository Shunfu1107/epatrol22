using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name:"IdentityModuleOrganize")]
    public class IdentityModuleOrganize
    {
        [Key]
        public long Pvid { get; set; }
        public long IdentityModulePvid { get; set; }
        public string? PermissionKey { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
