using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using System.Data;


namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UserProfilesByUserFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }

        public static implicit operator UserProfilesByUserFilter(AppUser user)
        {
            return new UserProfilesByUserFilter
            {
                UserPvid = user.UserID
            };
        }
        public static implicit operator UserProfilesByUserFilter(long userPvid)
        {
            return new UserProfilesByUserFilter
            {
                UserPvid = userPvid
            };
        }
        public static implicit operator UserProfilesByUserFilter(string userIdentifier)
        {
            var pvid = 0L;
            if (!long.TryParse(userIdentifier, out pvid))
            {
                throw new Exception("Unable to parse userIdentifier to persistence Id.");
            }

            return new UserProfilesByUserFilter
            {
                UserPvid = pvid
            };
        }
    }
}
