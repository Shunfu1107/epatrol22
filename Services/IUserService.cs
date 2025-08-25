using AdminPortalV8.Data;
using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Entities;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Services
{
    public interface IUserService
    {
        Task<List<User>> AuthenticateAsync(AuthenticateServiceModel serviceModel);

        Task<bool> UpdateNameForPersonByUserIdAsync(UpdateNameForPersonViewModel serviceModel);

        Task<bool> DeleteUserDetailByIdAsync(int UserId);
        Task<string> GetPhotoByUserId(int UserId);
        Task<bool> InsertPhotoByUserId(int UserId, string imagePath);
        Task<List<Person>> GetUserListByRoleNameAsync(string RoleName);
        Task<bool> RemoveActivation(int UserId);
        Task<bool> RestoreActivation(int UserId);
        Task<bool> CheckShortNameDuplicate(string ShortName);
        Task<bool> EditCheckShortNameDuplicate(EditCheckShortNameExistance model);

        Task<bool> CheckNRICDuplicate(string NRIC);

        Task<bool> AddUserProfiles(int userID, int cid);

        Task<ApplicationUser> GetApplicationUserByUsernameAndCid(string userName, int cid);
        Task<bool> AddApplicationUser(ApplicationUser user, string password);
        Task<bool> UpdateApplicationUser(ApplicationUser user);
        Task<int> GetUserInHQ(int userId, int cid);
    }

    public class UserServices : IUserService
    {
        private readonly ApplicationDbContext _context;
        //private readonly IHttpClientFactory _clientFactory; // for httpclient
        //private readonly IConfiguration _configuration; //for accessing appsettings.json
        //private readonly IHostingEnvironment _hostingEnvironment; //for accessing wwwroot
        private readonly IUser _user;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserServices(ApplicationDbContext context, IUser user, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            //_clientFactory = clientFactory;  //Exanoke if use: using (var client = _clientFactory.CreateClient()) { }
            //_configuration = configuration;  //Example of use: _configuration["ApiSettings:BaseAddress"]
            //_hostingEnvironment = hostingEnvironment; //Example of use: _hostingEnvironment.ContentRootPath
            _user = user;
            _roleManager = roleManager;
        }


        public async Task<bool> UpdateNameForPersonByUserIdAsync(UpdateNameForPersonViewModel serviceModel)
        {
            try
            {
                if (serviceModel != null)
                {
                    var person = _context.Persons.FirstOrDefault(p => p.UserId == serviceModel.UserId);
                    if (person != null)
                    {
                        person.Name = !String.IsNullOrWhiteSpace(serviceModel.Name) ? serviceModel.Name : person.Name;
                    }
                    _context.Persons.Update(person);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

         public async Task<bool> DeleteUserDetailByIdAsync(int UserId)
        {
            if (UserId >= 0)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == UserId);
                var person = _context.Persons.FirstOrDefault(p => p.UserId == UserId);


                if (user != null)
                {
                    _context.Users.Remove(user);
                }
               
                if (person != null)
                {
                    

                    _context.Persons.Remove(person);
                }
                

                
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetPhotoByUserId(int UserId)
        {
            if (UserId >= 0)
            {
                Person person = _context.Persons.FirstOrDefault(p => p.UserId == UserId);
                await Task.CompletedTask;
                if (person.PhotoPath != null)
                {
                    return person.PhotoPath;
                }
            }
            return null;
        }

        public async Task<bool> InsertPhotoByUserId(int UserId, string imagePath)
        {
            if (UserId >= 0)
            {
                Person person = _context.Persons.FirstOrDefault(p => p.UserId == UserId);
                person.PhotoPath = imagePath;

                _context.Update(person);
            }
            await _context.SaveChangesAsync();

            return true;
        }

      
        public async Task<List<Person>> GetUserListByRoleNameAsync(string RoleName)
        {
            await Task.CompletedTask;
            if (!string.IsNullOrEmpty(RoleName))
            {
                var Role = _roleManager.Roles.FirstOrDefault(r => r.Name.ToUpper() == RoleName.ToUpper());
                if (Role != null)
                {
                    var UserIds = _context.IdentityRoleProfiles.Where(p => p.IdentityRolePvid == Role.Id).Select(p => Convert.ToInt32(p.IdentityUserPvid));
                    List<Person> PersonList = new List<Person>();
                    if (UserIds.Count() == 0)
                    {
                        return PersonList;
                    }
                    else
                    {
                        foreach (var UserId in UserIds)
                        {
                            var person = _context.Persons.FirstOrDefault(pe => pe.UserId == UserId);
                            if (person != null)
                            {
                                PersonList.Add(person);
                            }
                        }
                    }
                    return PersonList;
                }
                throw new Exception("Role Name Not Exist");
            }
            throw new Exception("Role Name Not Found");
        }

        public async Task<bool> RemoveActivation(int UserId)
        {
            //var User = _roleManager.Roles.FirstOrDefault(u => u.Id == UserId);
            var User = _context.Users.FirstOrDefault(u => u.Id == UserId);


            User.Active = false;

            _context.Users.Update(User);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreActivation(int UserId)
        {
            var User = _context.Users.FirstOrDefault(u => u.Id == UserId);

            User.Active = true;

            _context.Users.Update(User);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckShortNameDuplicate(string ShortName)
        {
            var Exist = _context.Persons.Where(p => p.ShortName == ShortName).FirstOrDefault();

            await Task.CompletedTask;

            if (Exist != null) //short name exists
            {
                return false;
            }
            return true;
        }
        public async Task<bool> EditCheckShortNameDuplicate(EditCheckShortNameExistance model)
        {
            await Task.CompletedTask;
            if (model != null)
            {
                var existName = _context.Persons.FirstOrDefault(p => p.UserId == model.UserId && p.ShortName == model.ShortName);
                if (existName != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckNRICDuplicate(string NRIC)
        {
            try
            {
                await Task.CompletedTask;

                var result = _context.Persons.FirstOrDefault(p => p.NRIC == NRIC);
                if (result != null)
                {
                    return false;
                }
                else
                {

                    string last4Chars = NRIC.Substring(NRIC.Length - 5);
                    result = _context.Persons.FirstOrDefault(p => p.NRIC.EndsWith(last4Chars));
                    if (result != null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        Task<List<User>> IUserService.AuthenticateAsync(AuthenticateServiceModel serviceModel)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserProfiles(int userID, int cid)
        {
            try
            {
                var user = _context.IdentityUserProfiles.FirstOrDefault(p => p.IdentityUserPvid == userID);
                if (user == null)
                {
                    _context.IdentityUserProfiles.Add(new IdentityUserProfiles()
                    {
                        IdentityUserPvid = userID,
                        CID = cid

                    });

                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ApplicationUser> GetApplicationUserByUsernameAndCid(string userName, int cid)
        {
            try
            {
                var item = (from p in _context.Users
                            where p.NormalizedUserName == userName.ToUpper() &&
                            p.cid == cid
                            select new ApplicationUser()
                            {
                                Id = p.Id,
                                Active = p.Active,
                                AccessFailedCount = p.AccessFailedCount,
                                cid = p.cid,
                                ConcurrencyStamp = p.ConcurrencyStamp,
                                Email = p.Email,
                                EmailConfirmed = p.EmailConfirmed,
                                FirstPasswordReset = p.FirstPasswordReset,
                                LockoutEnabled = p.LockoutEnabled,
                                LockoutEnd = p.LockoutEnd,
                                Name = p.Name,
                                NormalizedEmail = p.NormalizedEmail,
                                NormalizedUserName = p.NormalizedUserName,
                                Password = p.Password,
                                PasswordHash = p.PasswordHash,
                                PhoneNumber = p.PhoneNumber,
                                PhoneNumberConfirmed = p.PhoneNumberConfirmed,
                                referId = p.referId,
                                SecurityStamp = p.SecurityStamp,
                                TwoFactorEnabled = p.TwoFactorEnabled,
                                UserName = p.UserName

                            }).FirstOrDefault();

                await Task.CompletedTask;

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddApplicationUser(ApplicationUser user, string password)
        {
            try
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var HashedPassword = passwordHasher.HashPassword(null, password);

                user.Password = HashedPassword;
                user.PasswordHash = HashedPassword;

                _context.Users.Add(user);
                _context.SaveChanges();

                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateApplicationUser(ApplicationUser user)
        {
            try
            {
                var item = _context.Users.FirstOrDefault(p => p.Id == user.Id);
                if (item != null)
                {
                    item.Password = user.PasswordHash;
                    _context.Update(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetUserInHQ(int userId, int cid)
        {
            try
            {
                var item = await _context.Users.FirstOrDefaultAsync(p => p.referId == userId.ToString() && p.cid == cid);

                if (item != null)
                {
                    return item.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
