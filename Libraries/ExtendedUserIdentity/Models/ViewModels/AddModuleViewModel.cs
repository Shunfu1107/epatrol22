using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class AddModuleViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}