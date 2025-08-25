using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteAccessLevelDetailFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityAccessLevelPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityModulePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }
    }
}
