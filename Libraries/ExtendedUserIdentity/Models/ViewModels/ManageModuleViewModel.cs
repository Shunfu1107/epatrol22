using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ManageModuleViewModel
    {
        [Required]
        public long ModulePvid { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}