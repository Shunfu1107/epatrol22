using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class AccessLevelDetailFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long AccessLevelPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }
    }
}
