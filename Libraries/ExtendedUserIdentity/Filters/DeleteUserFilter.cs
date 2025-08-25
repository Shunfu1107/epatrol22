using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteUserFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserID { get; set; }

        public static implicit operator DeleteUserFilter(AppUser user)
        {
            return new DeleteUserFilter
            {
                UserID = user.UserID
            };
        }
    }
}
