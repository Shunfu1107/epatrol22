using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppRoleAccess
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityRolePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityModulePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long IdentityAccessLevelPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long GrantedByIdentityUserPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm:ss")]
        public DateTime GrantedDate { get; set; }

        public AppAccessLevel? AccessLevel { get; set; }
        public AppModule? Module { get; set; }
        public AppUser? GrantedBy { get; set; }
    }
}
