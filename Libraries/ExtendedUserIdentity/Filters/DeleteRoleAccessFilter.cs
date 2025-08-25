using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteRoleAccessFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RoleAccessPvid { get; set; }

        public static implicit operator DeleteRoleAccessFilter(AppRoleAccess access)
        {
            return new DeleteRoleAccessFilter
            {
                RoleAccessPvid = access.Pvid
            };
        }
    }
}
