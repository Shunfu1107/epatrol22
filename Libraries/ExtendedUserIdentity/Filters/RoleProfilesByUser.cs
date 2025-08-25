using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;


namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class RoleProfilesByUser
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }

        public static implicit operator RoleProfilesByUser(AppUser user)
        {
            return new RoleProfilesByUser
            {
                UserPvid = user.UserID
            };
        }
    }
}
