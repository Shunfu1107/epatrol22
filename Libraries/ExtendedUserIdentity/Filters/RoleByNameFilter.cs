using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class RoleByNameFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public static implicit operator RoleByNameFilter(string roleName)
        {
            return new RoleByNameFilter
            {
                Name = roleName
            };
        }
    }
}
