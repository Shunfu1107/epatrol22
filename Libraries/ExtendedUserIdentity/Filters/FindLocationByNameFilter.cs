using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class FindLocationByNameFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.VarChar)]
        public string LocationName { get; set; }

        public static implicit operator FindLocationByNameFilter(string locName)
        {
            return new FindLocationByNameFilter
            {
                LocationName = locName
            };
        }
    }
}
