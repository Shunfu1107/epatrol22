using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class AddUserRoleViewModel
    {
        public long UserPvid { get; set; }
        [Required]
        [Display(Name = "Role")]
        public long RolePvid { get; set; }

        public IList<ApplicationRole>? Roles { get; set; }

        public static implicit operator AppRoleProfiles(AddUserRoleViewModel userrole)
        {
            return new AppRoleProfiles
            {
                IdentityRolePvid = userrole.RolePvid,
                IdentityUserPvid = userrole.UserPvid
            };
        }
    }
}