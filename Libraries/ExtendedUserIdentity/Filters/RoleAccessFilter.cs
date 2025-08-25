using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;


namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class RoleAccessFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RolePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long AccessLevelPvid { get; set; }

        public static implicit operator RoleAccessFilter(AppRoleAccess access)
        {
            return new RoleAccessFilter
            {
                AccessLevelPvid = access.IdentityAccessLevelPvid,
                ModulePvid = access.IdentityModulePvid,
                RolePvid = access.IdentityRolePvid
            };
        }
    }
}
