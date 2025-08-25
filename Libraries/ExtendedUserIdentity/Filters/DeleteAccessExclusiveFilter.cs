using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteAccessExclusiveFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ExclusivePvid { get; set; }

        public static implicit operator DeleteAccessExclusiveFilter(AppAccessLevelExclusive exclusive)
        {
            return new DeleteAccessExclusiveFilter
            {
                ExclusivePvid = exclusive.Pvid
            };
        }
    }
}
