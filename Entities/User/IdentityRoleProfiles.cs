using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class IdentityRoleProfiles
    {
        [Key]
        public long Pvid { get; set; }
        [ForeignKey("User")]
        public long IdentityUserPvid { get; set; }
        [ForeignKey("IdentityRole")]
        public long IdentityRolePvid { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }

    }
}