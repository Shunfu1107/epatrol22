using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    public class ApplicationRole : IdentityRole<int>
    {
        [Column("PvId", TypeName = "bigint")]
        public override int Id { get; set; }
        public bool isActive { get; set; }
        public DateTime StartActiveDate { get; set; }
        public DateTime EndActiveDate { get; set; }
    }
}
