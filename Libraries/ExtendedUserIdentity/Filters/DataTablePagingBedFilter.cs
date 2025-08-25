using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DataTablePagingBedFilter : DataTablePagingFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RoomPvid { get; set; }
    }
}
