using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class FindUserByUsernameFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Username { get; set; }

        public static implicit operator FindUserByUsernameFilter(string username)
        {
            return new FindUserByUsernameFilter
            {
                Username = username
            };
        }
    }
}
