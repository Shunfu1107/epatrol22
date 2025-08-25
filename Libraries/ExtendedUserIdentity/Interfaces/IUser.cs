using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Models;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces
{
    public interface IUser
    {
        AppUser GetUser(string username);
        bool UpdateActiveStatus(int id, bool active);
        AppUserProfiles GetUserProfiles(UserProfilesByUserFilter filter);
        void AddUserProfiles(AppUserProfiles profile);
        AppUser FindByID(FindUserByIDFilter filter);
        Paging<AppUser> GetUsers(DataTablePagingFilter filter);
        UserPasswordHistory GetUserPasswordHistory(int UserID);
        bool UpdatePasswordHistory(UserPasswordHistory pass);
        ApplicationUser GetUserByID(int id);

        Task DeleteUserDetailByIdAsync(int UserId);
        List<ApplicationUser> GetAllUsers();
    }
}
