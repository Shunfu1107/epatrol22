using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DataTablePagingUserRolesFilter : DataTablePagingFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }
    }
}
