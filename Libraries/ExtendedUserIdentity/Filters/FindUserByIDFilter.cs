using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class FindUserByIDFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long Pvid { get; set; }

        public static implicit operator FindUserByIDFilter(long userPvid)
        {
            return new FindUserByIDFilter
            {
                Pvid = userPvid
            };
        }
    }
}
