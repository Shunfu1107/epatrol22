using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteRoleProfileFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RolePvid { get; set; }
    }

    public sealed class DeleteRestaurantProfileFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RestPvid { get; set; }
    }
}
