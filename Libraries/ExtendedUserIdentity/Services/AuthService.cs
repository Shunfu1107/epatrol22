using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Services
{
    public class AuthService : IAuth
    {
        private readonly IUser userService;

        public AuthService(IUser user) 
        { 
            userService = user;
        }

        public AppUser GetUserByID(FindUserByIDFilter filter)
        {
            try
            {
                return userService.FindByID(filter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AppUserProfiles GetUserProfiles(UserProfilesByUserFilter filter)
        {
            try
            {
                return userService.GetUserProfiles(filter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Paging<AppUser> GetUsers(DataTablePagingFilter filter)
        {
            try
            {
                return userService.GetUsers(filter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public AppRole GetRole(RoleByNameFilter filter)
        //{
        //    try
        //    {
        //        return roleService.FindRole(filter);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public void DeleteRoleProfile(DeleteRoleProfileFilter filter)
        //{

        //    try
        //    {
        //        roleService.DeleteRoleProfile(filter);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
