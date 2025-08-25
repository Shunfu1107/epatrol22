using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteRoleFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RolePvid { get; set; }

        public static implicit operator DeleteRoleFilter(AppRole role)
        {
            return new DeleteRoleFilter
            {
                RolePvid = role.Pvid
            };
        }
    }
}
