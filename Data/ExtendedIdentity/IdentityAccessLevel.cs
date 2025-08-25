using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name: "IdentityAccessLevel")]
    public class IdentityAccessLevel
    {

        public long Pvid { get; set; }
        public string Name { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
