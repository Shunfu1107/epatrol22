using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces
{
    public interface IAuth
    {
        AppUser GetUserByID(FindUserByIDFilter filter);
        AppUserProfiles GetUserProfiles(UserProfilesByUserFilter filter);
        Paging<AppUser> GetUsers(DataTablePagingFilter filter);
    }
}
