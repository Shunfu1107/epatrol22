using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class AccessLevelByNameFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public static implicit operator AccessLevelByNameFilter(string accesslevelName)
        {
            return new AccessLevelByNameFilter
            {
                Name = accesslevelName
            };
        }
    }
}
