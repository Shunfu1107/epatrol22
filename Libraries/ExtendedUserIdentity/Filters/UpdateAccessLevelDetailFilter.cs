using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateAccessLevelDetailFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long BelongToModulePvid { get; set; }
    }
}
