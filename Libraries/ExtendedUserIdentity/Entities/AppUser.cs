using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppUser : IdentityUser<string>
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserID { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string UserName { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? Password { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? Name { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Email { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? SecurityStamp { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Bit)]
        public bool FirstPasswordReset { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Bit)]
        public bool Active { get; set; }

        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? NRIC { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? Mobile { get; set; }


        public string Id
        {
            get { return this.UserID.ToString(); }
        }

        public AppUserProfiles? UserProfiles { get; set; }

        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string ReceivedEmail { get; set; }
    }
}
