using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class AddExclusiveAccessViewModel
    {

        [Required]
        public long UserPvid { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Access Right")]
        public string PermissionKey { get; set; }

        [Required]
        public bool Accessible { get; set; }

        public IList<object> Accessibles { get; set; }

        public static implicit operator AppAccessLevelExclusive(AddExclusiveAccessViewModel access)
        {
            return new AppAccessLevelExclusive
            {
                Accessible = access.Accessible,
                IdentityUserPvid = access.UserPvid,
                PermissionKey = access.PermissionKey
            };
        }
    }
}