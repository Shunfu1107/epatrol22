using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppUserProfiles
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long UserPvid { get; set; }
        public IList<AppRoleProfiles> RoleProfiles { get; set; }
        public IList<AppAccessLevelExclusive> AccessLevelExclusiveList { get; set; }
    }

    public class UserObj
    {
        public AppUser user { get; set; }
        public AppUserProfiles userProfiles { get; set; }
        public List<AppPermission> permission { get; set; }
        public int RestaurantId { get; set; }

    }
}
