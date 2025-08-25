using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ManageAccessLevelViewModel
    {
        [Required]
        public long AccessLevelPvid { get; set; }

        [Required]
        [Display(Name = "Access Level")]
        public string Name { get; set; }
    }
}