using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class LocationByUserFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityUserPvid { get; set; }

        public static implicit operator LocationByUserFilter(string identifier)
        {
            var id = 0L;
            if (!long.TryParse(identifier, out id))
            {
                throw new InvalidOperationException("LocationByUserFilter(identifier)");
            }
            return new LocationByUserFilter
            {
                IdentityUserPvid = id
            };
        }
    }
}
