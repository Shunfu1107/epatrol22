using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ManageAccessLevelPermissionViewModel
    {
        public long AccessLevelPvid { get; set; }
        public long ModulePvid { get; set; }
        public string PermissionKey { get; set; }
        public bool Included { get; set; }

        public static implicit operator AppAccessLevelDetail(ManageAccessLevelPermissionViewModel model)
        {
            return new AppAccessLevelDetail
            {
                IdentityAccessLevelPvid = model.AccessLevelPvid,
                IdentityModulePvid = model.ModulePvid,
                PermissionKey = model.PermissionKey
            };
        }
        public static implicit operator DeleteAccessLevelDetailFilter(ManageAccessLevelPermissionViewModel model)
        {
            return new DeleteAccessLevelDetailFilter
            {
                IdentityAccessLevelPvid = model.AccessLevelPvid,
                IdentityModulePvid = model.ModulePvid,
                PermissionKey = model.PermissionKey
            };
        }
    }
}