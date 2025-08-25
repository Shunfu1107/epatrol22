using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ProfileViewModel
    {
        public long UserID { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password *")]
        [Required]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\\d)(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Please follow the password specification")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password *")]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password *")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}