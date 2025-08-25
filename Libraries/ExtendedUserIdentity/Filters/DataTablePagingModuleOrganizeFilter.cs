using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DataTablePagingModuleOrganizeFilter : DataTablePagingFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }
    }
}
