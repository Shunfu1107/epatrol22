using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppPermission
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityRolePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }
    }
}
