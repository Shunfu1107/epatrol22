using Microsoft.AspNetCore.Identity;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    public class ApplicationUser : IdentityUser <int>
    {
        //public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public bool Active { get; set; }
        public bool? FirstPasswordReset { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? NormalizedEmail { get; set; }
        public int? cid { get; set; }
        public string? referId { get; set; }

        public bool ReceivedEmail { get; set; }


    }
}
