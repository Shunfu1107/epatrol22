using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class FindUserByEmailFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.VarChar)]
        public string Email { get; set; }

        public static implicit operator FindUserByEmailFilter(string email)
        {
            return new FindUserByEmailFilter
            {
                Email = email
            };
        }
    }
}
