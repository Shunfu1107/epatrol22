using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;

namespace AdminPortalV8.Models.User
{
    public class ConfigModel
    {
        public AppUser User { get; set; }
        public AppUserProfiles UserProfiles { get; set; }
        public List<PermissionData> Permission { get; set; }
    }
}
