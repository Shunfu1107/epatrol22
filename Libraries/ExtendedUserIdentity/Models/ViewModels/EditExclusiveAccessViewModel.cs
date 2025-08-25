using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class EditExclusiveAccessViewModel
    {
        public long UserPvid { get; set; }

        public long ExclusivePvid { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Access Right")]
        public string PermissionKey { get; set; }

        [Required]
        public bool Accessible { get; set; }

        public IList<object> Accessibles { get; set; }

        public static implicit operator AppAccessLevelExclusive(EditExclusiveAccessViewModel access)
        {
            return new AppAccessLevelExclusive
            {
                Pvid = access.ExclusivePvid,
                Accessible = access.Accessible,
                IdentityUserPvid = access.UserPvid,
                PermissionKey = access.PermissionKey
            };
        }
    }
}