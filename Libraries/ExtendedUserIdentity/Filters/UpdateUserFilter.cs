using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateUserFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string UserName { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string PasswordHash { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string SecurityStamp { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Email { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public static implicit operator UpdateUserFilter(AppUser user)
        {
            return new UpdateUserFilter
            {
                UserId = user.UserID,
                UserName = user.UserName,
                PasswordHash = user.Password,
                SecurityStamp = user.SecurityStamp,
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}
