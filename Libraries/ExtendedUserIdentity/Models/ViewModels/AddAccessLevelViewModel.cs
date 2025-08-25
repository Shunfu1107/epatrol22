using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class AddAccessLevelViewModel
    {
        [Required]
        [Display(Name = "Access Level")]
        public string Name { get; set; }
    }
}