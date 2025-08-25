using AdminPortalV8.Models;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class PortalAdminViewModel
    {
        public LoginViewModel? LoginViewModel { get; set; }
        public RegisterViewModel? RegisterViewModel { get; set; }
        public ManageUserViewModel? ManageUserViewModel { get; set; }
        public AddUserRoleViewModel? AddUserRole { get; set; }
        public AddExclusiveAccessViewModel? AddExclusiveAccess { get; set; }
        public EditExclusiveAccessViewModel? EditExclusiveAccess { get; set; }
        public AddUserRestaurant? AddUserRestaurant { get; set; }
    }
}