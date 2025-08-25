using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppRoleProfiles
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityUserPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityRolePvid { get; set; }

        public AppRole Role { get; set; }
    }
}
