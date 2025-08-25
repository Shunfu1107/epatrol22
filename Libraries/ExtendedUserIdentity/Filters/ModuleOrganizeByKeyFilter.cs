using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class ModuleOrganizeByKeyFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PermissionKey { get; set; }

        public static implicit operator ModuleOrganizeByKeyFilter(string permissionKey)
        {
            return new ModuleOrganizeByKeyFilter
            {
                PermissionKey = permissionKey
            };
        }

    }
}
