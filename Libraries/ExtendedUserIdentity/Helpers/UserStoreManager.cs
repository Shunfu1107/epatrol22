using AdminPortalV7.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers
{
    public class UserStoreManager : IUserStore<AppUser>, IUserLoginStore<AppUser>, IUserPasswordStore<AppUser>, IUserSecurityStampStore<AppUser>, IUserEmailStore<AppUser>
    {
        private readonly AuthService service;

        public UserStoreManager()
        {
            service = new AuthService();
        }

        #region IUserSecurityStampStore
        public Task<string> GetSecurityStampAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }
        #endregion

        #region IUserPasswordStore
        public Task<string> GetPasswordHashAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash)
        {
            if (user == null) throw new ArgumentNullException("user");
            user.Password = passwordHash;
            return Task.FromResult(0);
        }
        #endregion

        #region IUserLoginStore
        public Task AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(AppUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserStore
        public Task CreateAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                service.AddUser(user);
            });
        }
        public Task DeleteAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.Factory.StartNew(() =>
            {
                service.DeleteUser(user);
            });
        }
        public Task<AppUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");
            long parsedUserId;
            if (!long.TryParse(userId, out parsedUserId)) throw new ArgumentOutOfRangeException(string.Format("'{0}' is not a valid User ID.", userId));

            return Task.Factory.StartNew(() =>
            {
                return service.GetUserByID(parsedUserId);
            });
        }
        public Task<AppUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException("userName");
            return Task.Factory.StartNew(() =>
            {
                return service.GetUserByUserName(userName);
            });
        }
        public Task UpdateAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Task.Factory.StartNew(() =>
            {
                service.UpdateUser(user);
            });
        }
        public Task<AppUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException("email");
            if (string.IsNullOrEmpty(email)) throw new ArgumentOutOfRangeException(string.Format("'{0}' is not a valid email.", email));

            return Task.Factory.StartNew(() =>
            {
                return service.GetUserByEmail(email);
            });
        }

        public void Dispose()
        {

        }

        public Task SetEmailAsync(AppUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}