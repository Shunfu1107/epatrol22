using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Models.User
{
    public class AuthenticateServiceModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
