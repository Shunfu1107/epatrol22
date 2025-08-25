using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}