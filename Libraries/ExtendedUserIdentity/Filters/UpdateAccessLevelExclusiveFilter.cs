using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateAccessLevelExclusiveFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ExclusivePvid { get; set; }

        [ParameterizeQuery(DbType = SqlDbType.Bit)]
        public bool Accessible { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }


        public static implicit operator UpdateAccessLevelExclusiveFilter(AppAccessLevelExclusive exclusive)
        {
            return new UpdateAccessLevelExclusiveFilter
            {
                ExclusivePvid = exclusive.Pvid,
                Accessible = exclusive.Accessible,
                PermissionKey = exclusive.PermissionKey
            };
        }

    }
}
