using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Username must not be more than {1} characters long.")]
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Required]
        [Display(Name = "Full Name (as in NRIC)")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\\d)(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Please follow the password specification")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string[]? Role { get; set; }
        public string? EzlinkCard { get; internal set; }
        public int UserId { get; internal set; }
        //public string? Mobile { get; internal set; }
        //public string? NRIC { get; internal set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Is Receive Email")]
        public bool ReceivedEmail { get; set; }
    }

   
}