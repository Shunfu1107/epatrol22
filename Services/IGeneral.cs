using AdminPortalV8.Data;
using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Helpers;
using System.Security.Cryptography;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Reflection;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Models.User;
using static iTextSharp.text.pdf.AcroFields;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AdminPortalV8.Services
{
    public interface IGeneral
    {
        Task<bool> UpdateNameForPersonByUserIdAsync(UpdateNameForPersonViewModel serviceModel);
        ApplicationUser GetUserByID(int id);
        Task<Paging<AppUser>> GetAllUsers(DataTablePagingFilter filter);
        AppUser GetUserByUserName(string userName);
        AppUser GetUserByEmail(string Email);
        UserPasswordHistory GetUserPasswordHistory(int userID);
        bool UpdatePasswordHistory(UserPasswordHistory userPasswordHistory);
        bool UpdateActiveStatus(int id, bool active);
        bool UpdateReceivedEmail(string userName, bool receivedEmail);
        Task<Paging<AppRole>> GetUserRoles(int UserID);
        Task<bool> RemoveRoleUserProfiles(int roleID, int userID);
        Task<Paging<AppAccessLevelExclusive>> GetUserAccessExclusive(int UserID);
        Task<bool> AddUserRoles(AddUserRoleViewModel viewModel);
        bool CheckExistingUserRole(ApplicationUser user, ApplicationRole role);
        List<AppModule> GetModuleWithPermissionList();
        Task<bool> AddExclusiveAccess(AddExclusiveAccessViewModel viewModel);
        bool CheckExistingExclusive(int userID, string key, int pvid);
        AppAccessLevelExclusive GetAccessExclusiveByID(int exclusiveID);
        Task<bool> UpdateExclusiveAccess(EditExclusiveAccessViewModel viewModel);
        Task<bool> RemoveAccessExclusive(int pvid);
        bool DeleteUserPerson(int userID);
        List<PermissionData> GetAllContentPermissions();

        Task<bool> UpdateFirstPassword(int userId);

        //MODULE
        Task<Paging<AppModule>> GetModules(DataTablePagingFilter filter);
        Task<int> GetModulesInHQ(int referId, int cid);
        Task<bool> AddModule(AppModule viewModel);
        Task<bool> UpdateModule(AppModule viewModel);
        bool DeleteModule(AppModule viewModel);
        Task<Paging<AppModuleOrganize>> GetModulesOrganize(DataTablePagingModuleOrganizeFilter filter);
        bool AddModuleOrganize(AppModuleOrganize viewModel);
        bool DeleteModuleOrganize(DeleteModuleOrganizeFilter viewModel);

        //ROLE
        List<AppModule> GetAllActiveModule();
        List<AppAccessLevel> GetAllAccessLevel();
        Task<bool> AddRole(AppRole role);
        Task<bool> UpdateRole(AppRole role);
        bool DeleteRole(AppRole role);
        Task<bool> AddRoleAccess(AppRoleAccess accessrights);
        bool DeleteRoleAccess(AppRoleAccess accessrights);
        Task<Paging<AppRole>> GetRoles(DataTablePagingFilter filter);
        Task<Paging<AppRoleAccess>> GetRoleAccessDetails(DataTablePagingRoleAccessFilter filter);

        //ACCESS LEVEL
        Task<bool> AddAccessLevel(AppAccessLevel access);
        Task<bool> UpdateAccessLevel(AppAccessLevel access);
        Task<bool> DeleteAccessLevel(DeleteAccessLevelFilter filter);
        Task<Paging<AppAccessLevel>> GetAccessLevels(DataTablePagingFilter filter);
        Task<bool> AddAccessLevelDetail(AppAccessLevelDetail detail);
        Task<bool> DeleteAccessLevelDetail(DeleteAccessLevelDetailFilter filter);
        Task<Paging<AppAccessLevelDetail>> GetAccessLevelDetails(DataTablePagingAccessDetailsFilter filter);
        IList<AppModuleOrganize> GetModuleOrganizes(ModuleIDFilter filter);

        Task<List<string>> GetPermissionDefault(int userId);
        //ANALYCTICS
        Task<Paging<AppRobot>> GetRobot(DataTablePagingFilter filter);
    }

    public class GeneralService : IGeneral
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public GeneralService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }

        public List<PermissionData> GetAllContentPermissions()
        {
            var permissions = new List<ContentPermissionAttribute>();
            var permissionList = new List<PermissionData>();

            // Use reflection to find methods with the ContentPermissionAttribute
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.GetName().Name.Contains("System.Web"))
                {
                    var methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(ContentPermissionAttribute), false).Length > 0)
                    .ToList();

                    foreach (var method in methods)
                    {
                        var attribute = (ContentPermissionAttribute)method.GetCustomAttribute(typeof(ContentPermissionAttribute));
                        permissions.Add(attribute);

                        
                        permissionList.Add(new PermissionData()
                        {
                            Key = attribute.Key,
                            Title = attribute.Title,
                            Description = attribute.Desc,
                            StaticAuthorized = attribute.StaticAuthorization,
                            AssociatedKey = attribute.AssociatedKey
                            
                        });
                    }
                }

            }

            return permissionList;
        }


        public async Task<bool> AddExclusiveAccess(AddExclusiveAccessViewModel viewModel)
        {
            try
            {
                _context.IdentityAccessLevelExclusives.Add(new IdentityAccessLevelExclusive()
                {
                    IdentityUserPvid = viewModel.UserPvid,
                    Accessible = viewModel.Accessible,
                    PermissionKey = viewModel.PermissionKey
                });

                _context.SaveChanges();
                await Task.CompletedTask;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddUserRoles(AddUserRoleViewModel viewModel)
        {
            try
            {
                _context.IdentityRoleProfiles.Add(new IdentityRoleProfiles()
                {
                    IdentityRolePvid = viewModel.RolePvid,
                    IdentityUserPvid = viewModel.UserPvid

                });

                _context.SaveChanges();
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckExistingExclusive(int userID, string key, int pvid)
        {
            var result = false;

            try
            {
                    var item = _context.IdentityAccessLevelExclusives.FirstOrDefault(p => p.IdentityUserPvid == userID && p.PermissionKey == key);

                    if (item != null)
                    {
                        if(pvid != 0)
                        {
                            if(pvid != item.Pvid)
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            result = true;
                        }
                        
                    }
                
               
            }
            catch(Exception ex)
            {
                
            }

            return result;
        }

        public bool CheckExistingUserRole(ApplicationUser user, ApplicationRole role)
        {
            var result = false;
            try
            {
                var item = _context.IdentityRoleProfiles.FirstOrDefault(p => p.IdentityRolePvid == role.Id && p.IdentityUserPvid == user.Id);

                if(item != null)
                {
                    result = true;
                }

                
            }
            catch (Exception ex)
            {
                
            }

            return result;
        }

        public bool DeleteUserPerson(int userID)
        {
            try
            {
                var item = _context.Persons.FirstOrDefault(p => p.UserId == userID);
                if(item != null)
                {
                    _context.Remove(item);

                    var rolesProf = _context.IdentityRoleProfiles.Where(p => p.IdentityUserPvid == userID).ToList();

                    foreach(var data in rolesProf)
                    {
                        _context.Remove(data);
                    }

                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public AppAccessLevelExclusive GetAccessExclusiveByID(int exclusiveID)
        {
            var result = new AppAccessLevelExclusive();
            try
            {
                result = (from p in _context.IdentityAccessLevelExclusives
                          where p.Pvid == exclusiveID
                          select new AppAccessLevelExclusive()
                          {
                              Pvid = p.Pvid,
                              IdentityUserPvid = p.IdentityUserPvid,
                              PermissionKey = p.PermissionKey,
                              Accessible = p.Accessible
                          }).FirstOrDefault();
            }
            catch(Exception ex)
            {

            }

            return result;
        }

        public async Task<Paging<AppUser>> GetAllUsers(DataTablePagingFilter filter)
        {
            var result = new List<AppUser>();
            var dto = new Paging<AppUser>();
            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var uu = _httpContextAccessor.HttpContext.User;

                var systemAuthorized = uu.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.SystemPrimaryKey);
                var adminAuthorized = uu.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.AdminPrimaryKey);

                if (systemAuthorized)
                {
                    result = await (from U in _context.Users
                                    join P in _context.Persons on U.Id equals P.UserId into joinGroup
                                    from C in joinGroup.DefaultIfEmpty()
                                    where U.Active == true
                                    select new AppUser()
                                    {
                                        UserID = U.Id,
                                        UserName = U.UserName,
                                        Name = U.Name,
                                        Email = U.Email,
                                        NRIC = C.NRIC ?? "",
                                        PhoneNumber = U.PhoneNumber,
                                        Active = U.Active,
                                        Mobile = C.Mobile ?? "",
                                        ReceivedEmail = U.ReceivedEmail == true ? "Yes" : "No"

                                    }).ToListAsync();
                }
                else if(adminAuthorized)
                {
                    result = await (from U in _context.Users
                                    join P in _context.Persons on U.Id equals P.UserId into joinGroup
                                    from C in joinGroup.DefaultIfEmpty()
                                    where U.Active == true && !_context.IdentityRoleProfiles
                                    .Join(_context.Roles, I => I.IdentityRolePvid, R => R.Id, (I, R) => new { I, R })
                                    .Where(j => j.I.IdentityUserPvid == U.Id && j.R.Name == "SYSTEM ADMIN")
                                    .Any()
                                    select new AppUser()
                                    {
                                        UserID = U.Id,
                                        UserName = U.UserName,
                                        Name = U.Name,
                                        Email = U.Email,
                                        NRIC = C.NRIC ?? "",
                                        PhoneNumber = U.PhoneNumber,
                                        Active = U.Active,
                                        Mobile = C.Mobile ?? "",
                                        ReceivedEmail = U.ReceivedEmail == true ? "Yes" : "No"

                                    }).ToListAsync();
                }
                else
                {
                    result = await (from U in _context.Users
                                    join P in _context.Persons on U.Id equals P.UserId into joinGroup
                                    from C in joinGroup.DefaultIfEmpty()
                                    where U.Active == true && !_context.IdentityRoleProfiles
                                    .Join(_context.Roles, I => I.IdentityRolePvid, R => R.Id, (I, R) => new { I, R })
                                    .Where(j => j.I.IdentityUserPvid == U.Id && j.R.Name == "SYSTEM ADMIN")
                                    .Any()
                                    select new AppUser()
                                    {
                                        UserID = U.Id,
                                        UserName = U.UserName,
                                        Name = U.Name,
                                        Email = U.Email,
                                        NRIC = C.NRIC ?? "",
                                        PhoneNumber = U.PhoneNumber,
                                        Active = U.Active,
                                        Mobile = C.Mobile ?? "",
                                        ReceivedEmail = U.ReceivedEmail == true ? "Yes" : "No"

                                    }).ToListAsync();
                }

                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    result = result.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper()) || p.NRIC.ToUpper().Contains(filter.sSearch.ToUpper()) || p.Mobile.Contains(filter.sSearch) || p.UserName.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = result.Count();

                result = result.OrderBy(p => p.Name).Skip(start).Take(end).ToList();




                if (result != null)
                {
                    dto.List = result;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

               
            }
            catch(Exception ex)
            {

            }

            return dto;
        }

        public List<AppModule> GetModuleWithPermissionList()
        {
            var result = new List<AppModule>();
            try
            {
                result = (from m in _context.IdentityModules
                          select new AppModule()
                          {
                              Pvid = m.Pvid,
                              Name = m.Name,
                              Permissions = (from om in _context.IdentityModuleOrganizes
                                             where om.IdentityModulePvid == m.Pvid
                                             select new AppModuleOrganize()
                                             {
                                                 Pvid = om.Pvid,
                                                 PermissionKey = om.PermissionKey,
                                                 IdentityModulePvid = om.IdentityModulePvid
                                             }).ToList()
                          }).ToList();
            }
            catch(Exception ex)
            {

            }

            return result;
        }

        public async Task<Paging<AppAccessLevelExclusive>> GetUserAccessExclusive(int UserID)
        {
            var dto = new Paging<AppAccessLevelExclusive>();

            try
            {
                var item = await (from p in _context.IdentityAccessLevelExclusives
                                  join u in _context.Users on p.IdentityUserPvid equals u.Id
                                  where u.Id == UserID
                                  select new AppAccessLevelExclusive()
                                  {
                                      Pvid = p.Pvid,
                                      IdentityUserPvid = p.IdentityUserPvid,
                                      PermissionKey = p.PermissionKey,
                                      Accessible = p.Accessible
                                  }).ToListAsync();

                dto.List = item;
            }
            catch(Exception ex)
            {

            }

            return dto;
        }

        public ApplicationUser GetUserByID(int id)
        {
            var user = new ApplicationUser();
            try
            {
                var data = (from U in _context.Users
                            join P in _context.Persons on U.Id equals P.UserId into joinGroup
                            from C in joinGroup.DefaultIfEmpty()
                            where U.Id == id 
                            select new ApplicationUser()
                            {
                                Id = U.Id,
                                UserName = U.UserName,
                                Password = U.Password,
                                SecurityStamp = U.SecurityStamp,
                                Name = U.Name,
                                Email = U.Email,
                                Active = U.Active,
                                FirstPasswordReset = U.FirstPasswordReset ?? false,
                                PasswordHash = U.PasswordHash
                            }).FirstOrDefault();

                user = data;
                
            }
            catch(Exception ex)
            {
                
            }

            return user;
        }

        public AppUser GetUserByUserName(string userName)
        {
            var user = new AppUser();
            try
            {
                user = (from U in _context.Users
                        where U.UserName == userName
                        select new AppUser()
                        {
                            UserID = U.Id,
                            UserName = U.UserName,
                            Password = U.Password,
                            SecurityStamp = U.SecurityStamp,
                            Name = U.Name,
                            Email = U.Email,
                            Active = U.Active,
                            FirstPasswordReset = U.FirstPasswordReset ?? false
                        }).FirstOrDefault();
            }
            catch(Exception ex)
            {

            }

            return user;
        }

        public AppUser GetUserByEmail(string Email)
        {
            var user = new AppUser();
            try
            {
                user = (from U in _context.Users
                        where U.Email == Email
                        select new AppUser()
                        {
                            UserID = U.Id,
                            UserName = U.UserName,
                            Password = U.Password,
                            SecurityStamp = U.SecurityStamp,
                            Name = U.Name,
                            Email = U.Email,
                            Active = U.Active,
                            FirstPasswordReset = U.FirstPasswordReset ?? false
                        }).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            return user;
        }

        public UserPasswordHistory GetUserPasswordHistory(int userID)
        {
            var result = new UserPasswordHistory();
            try
            {
                result = (from p in _context.UserPasswordsHistory
                          where p.UserID == userID
                          select new UserPasswordHistory()
                          {
                              UserID = p.UserID,
                              PasswordHash1 = p.PasswordHash1 ?? "",
                              PasswordDate1 = p.PasswordDate1,
                              PasswordHash2 = p.PasswordHash2 ?? "",
                              PasswordDate2 = p.PasswordDate2,
                              PasswordHash3 = p.PasswordHash3 ?? "",
                              PasswordDate3 = p.PasswordDate3,
                              PasswordHash4 = p.PasswordHash4 ?? "",
                              PasswordDate4 = p.PasswordDate4,
                              PasswordDate5 = p.PasswordDate5,
                              PasswordHash5 = p.PasswordHash5 ?? "",
                              PasswordHash6 = p.PasswordHash6 ?? "",
                              PasswordDate6 = p.PasswordDate6,
                              PasswordHash7 = p.PasswordHash7 ?? "",
                              PasswordDate7 = p.PasswordDate7
                          }).FirstOrDefault();
            }
            catch(Exception ex)
            {

            }

            return result;
        }

        public async Task<Paging<AppRole>> GetUserRoles(int UserID)
        {
            var dto = new Paging<AppRole>();
            try
            {
                var result = await (from I in _context.IdentityRoleProfiles
                                    join R in _context.Roles on I.IdentityRolePvid equals R.Id
                                    where I.IdentityUserPvid == UserID
                                    select new AppRole()
                                    {
                                        Pvid = R.Id,
                                        IsActive = R.isActive,
                                        Name = R.Name,
                                        StartActiveDate = R.StartActiveDate,
                                        EndActiveDate = R.EndActiveDate,
                                        username = _context.Users.FirstOrDefault(p => p.Id == UserID).UserName

                                    }).ToListAsync();

                dto.List = result;
            }
            catch (Exception ex)
            {

            }
            return dto;
        }

        public async Task<Paging<AppModuleOrganize>> GetModulesOrganize(DataTablePagingModuleOrganizeFilter filter)
        {
            var dto = new Paging<AppModuleOrganize>();

            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await (from p in _context.IdentityModuleOrganizes
                                   where p.IdentityModulePvid == filter.ModulePvid
                                   select new AppModuleOrganize()
                                   {
                                       Pvid = p.Pvid,
                                       IdentityModulePvid = p.IdentityModulePvid,
                                       PermissionKey = p.PermissionKey

                                   }).ToListAsync();
                //var total = items.Count();



                //if (!string.IsNullOrWhiteSpace(filter.sSearch))
                //{
                //    items = items.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                //}

                var total = items.Count();

                items = items.OrderBy(p => p.PermissionKey).Skip(start).Take(end).ToList();



                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

            }
            catch (Exception ex)
            {

            }

            return dto;
        }


        public async Task<bool> RemoveAccessExclusive(int pvid)
        {
            try
            {
                var item = _context.IdentityAccessLevelExclusives.FirstOrDefault(p => p.Pvid == pvid);
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleUserProfiles(int roleID, int userID)
        {
            try
            {
                var item = await (from up in _context.IdentityRoleProfiles
                                  where up.IdentityRolePvid == roleID && up.IdentityUserPvid == userID
                                  select up).FirstOrDefaultAsync();
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool UpdateActiveStatus(int id, bool active)
        {
            try
            {
                var item = GetUserByID(id);

                if(item != null)
                {
                    item.Active = active;

                    var result = _userManager.UpdateAsync(item);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool UpdateReceivedEmail(string userName, bool receivedEmail)
        {
            try
            {
                var item = _context.Users.FirstOrDefault(p => p.UserName == userName);

                if (item != null)
                {
                    item.ReceivedEmail = receivedEmail;
                    item.Password = item.PasswordHash;
                    //var result = _userManager.UpdateAsync(item);
                    _context.Update(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        public async Task<bool> UpdateExclusiveAccess(EditExclusiveAccessViewModel viewModel)
        {
            try
            {
                var item = _context.IdentityAccessLevelExclusives.FirstOrDefault(p => p.Pvid == viewModel.ExclusivePvid);
                if(item != null)
                {
                    item.PermissionKey = viewModel.PermissionKey;
                    item.Accessible = viewModel.Accessible;

                    _context.Update(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateNameForPersonByUserIdAsync(UpdateNameForPersonViewModel serviceModel)
        {
            try
            {
                if (serviceModel != null)
                {
                    var item = _context.Persons.FirstOrDefault(p=>p.UserId == serviceModel.UserId);

                    if(item != null)
                    {
                        item.Name = serviceModel.Name;

                        _context.Persons.Update(item);
                        _context.SaveChanges();
                        await Task.CompletedTask;
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }

        public bool UpdatePasswordHistory(UserPasswordHistory userPasswordHistory)
        {
            try
            {
                if (userPasswordHistory != null)
                {
                    var item = _context.UserPasswordsHistory.FirstOrDefault(p=>p.UserID == userPasswordHistory.UserID);
                    if(item != null)
                    {
                        if(item.PasswordHash6 != null)
                        {
                            item.PasswordHash7 = item.PasswordHash6;
                            item.PasswordDate7 = item.PasswordDate6;
                        }
                        if(item.PasswordHash5 != null)
                        {
                            item.PasswordHash6 = item.PasswordHash5;
                            item.PasswordDate6 = item.PasswordDate5;
                        }
                        if (item.PasswordHash4 != null)
                        {
                            item.PasswordHash5 = item.PasswordHash4;
                            item.PasswordDate5 = item.PasswordDate4;
                        }
                        if (item.PasswordHash3 != null)
                        {
                            item.PasswordHash4 = item.PasswordHash3;
                            item.PasswordDate4 = item.PasswordDate3;
                        }
                        if (item.PasswordHash2 != null)
                        {
                            item.PasswordHash3 = item.PasswordHash2;
                            item.PasswordDate3 = item.PasswordDate2;
                        }
                        if (item.PasswordHash1 != null)
                        {
                            item.PasswordHash2 = item.PasswordHash1;
                            item.PasswordDate2 = item.PasswordDate1;
                        }

                        item.PasswordHash1 = userPasswordHistory.PasswordHash1;
                        item.PasswordDate1 = userPasswordHistory.PasswordDate1;

                        _context.Update(item);

                    }
                    else
                    {
                        _context.UserPasswordsHistory.Add(new UserPasswordsHistory()
                        {
                            UserID = userPasswordHistory.UserID,
                            PasswordHash1 = userPasswordHistory.PasswordHash1,
                            PasswordDate1 = userPasswordHistory.PasswordDate1,

                        });
                    }
                    

                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<Paging<AppModule>> GetModules(DataTablePagingFilter filter)
        {
            var dto = new Paging<AppModule>();

            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await (from p in _context.IdentityModules
                                   select new AppModule()
                                   {
                                       Pvid = p.Pvid,
                                       Name = p.Name,
                                       Permissions = (from q in _context.IdentityModuleOrganizes
                                                      where q.IdentityModulePvid == p.Pvid
                                                      select new AppModuleOrganize()
                                                      {
                                                          Pvid = q.Pvid,
                                                          IdentityModulePvid = q.IdentityModulePvid,
                                                          PermissionKey = q.PermissionKey
                                                      }).ToList()
                                   }).ToListAsync();
                //var total = items.Count();
                

              
                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    items = items.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = items.Count();

                items = items.OrderBy(p=>p.Name).Skip(start).Take(end).ToList();

                //var tempitems = items.Select((app, index) => new
                //{
                //    RowIndex = index,
                //    app.Pvid,
                //    app.Name,
                //    app.Permissions
                //}).ToList()
                //.Skip(start)
                //.Take(end);

                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

            }
            catch (Exception ex)
            {

            }

            return dto;
        }

        public async Task<bool> AddModule(AppModule viewModel)
        {
            try
            {
                _context.IdentityModules.Add(new IdentityModule()
                {
                    Name = viewModel.Name
                });

                _context.SaveChanges();
                await Task.CompletedTask;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateModule(AppModule viewModel)
        {
            try
            {
                var item = _context.IdentityModules.FirstOrDefault(p=>p.Pvid == viewModel.Pvid);
                if(item != null)
                {
                    item.Name = viewModel.Name;

                    _context.Update(item);
                }

                await _context.SaveChangesAsync();

                return true;
               
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DeleteModule(AppModule viewModel)
        {
            try
            {
                var item = _context.IdentityModules.FirstOrDefault(p=>p.Pvid == viewModel.Pvid);

                if(item != null)
                {
                    _context.Remove(item);
                }

                _context.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<Paging<AppRobot>> GetRobot(DataTablePagingFilter filter)
        {
            var dto = new Paging<AppRobot>();

            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await(from p in _context.Robots
                                  where p.robot_serialnum == filter.robot_serialnum
                                  select new AppRobot()
                                  {
                                      robot_id = p.robot_id,
                                      robot_serialnum = p.robot_serialnum,
                                      created_datetime = p.created_datetime

                                  }).ToListAsync();
                //var total = items.Count();



                //if (!string.IsNullOrWhiteSpace(filter.sSearch))
                //{
                //    items = items.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                //}

                var total = items.Count();

                items = items.OrderBy(p => p.PermissionKey).Skip(start).Take(end).ToList();

               

                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

            }
            catch (Exception ex)
            {

            }

            return dto;
        }

        public bool AddModuleOrganize(AppModuleOrganize viewModel)
        {
            try
            {
                var item = _context.IdentityModuleOrganizes.FirstOrDefault(p=>p.PermissionKey == viewModel.PermissionKey && p.IdentityModulePvid == viewModel.IdentityModulePvid);
                if(item == null)
                {
                    _context.IdentityModuleOrganizes.Add(new IdentityModuleOrganize()
                    {
                        IdentityModulePvid = viewModel.IdentityModulePvid,
                        PermissionKey = viewModel.PermissionKey
                    });

                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DeleteModuleOrganize(DeleteModuleOrganizeFilter viewModel)
        {
            try
            {
                var item = _context.IdentityModuleOrganizes.FirstOrDefault(p => p.IdentityModulePvid == viewModel.IdentityModulePvid && p.PermissionKey == viewModel.PermissionKey);
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AppModule> GetAllActiveModule()
        {
            var list = new List<AppModule>();
            try
            {
                list = (from m in _context.IdentityModules
                        select new AppModule()
                        {
                            Pvid = m.Pvid,
                            Name = m.Name,
                        }).ToList();
            }
            catch(Exception ex)
            {

            }

            return list;
        }

        public List<AppAccessLevel> GetAllAccessLevel()
        {
            var list = new List<AppAccessLevel>();
            try
            {
                list = (from al in _context.IdentityAccessLevels
                        select new AppAccessLevel()
                        {
                            Pvid = al.Pvid,
                            Name = al.Name
                        }).ToList();
            }
            catch(Exception ex)
            {

            }

            return list;
        }

        public async Task<bool> AddRole(AppRole role)
        {
            try
            {
                var existrole = await _roleManager.FindByNameAsync(role.Name); 
                if (existrole != null && !existrole.Id.Equals(0) && !existrole.Id.Equals(role.Pvid))
                    throw new Exception("Role name has already existed.");

                _context.Roles.Add(new ApplicationRole()
                {
                    Name = role.Name,
                    NormalizedName = role.Name.ToUpper(),
                    StartActiveDate = role.StartActiveDate.Date,
                    EndActiveDate = role.EndActiveDate.Date,
                    isActive = true
                });

                _context.SaveChanges();
                await Task.CompletedTask;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateRole(AppRole role)
        {
            try
            {
                var existrole = await _roleManager.FindByNameAsync(role.Name);
                if (existrole != null && !existrole.Id.Equals(0) && !existrole.Id.Equals(role.Pvid))
                    throw new Exception("Role name has already existed.");

                var item = _context.Roles.FirstOrDefault(x => x.Id == role.Pvid);
                if(item != null)
                {
                    item.Name = role.Name;
                    item.NormalizedName = role.Name.ToUpper();
                    item.StartActiveDate = role.StartActiveDate;
                    item.EndActiveDate = role.EndActiveDate;

                    _context.Update(item);
                    _context.SaveChanges();
                }

                

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteRole(AppRole role)
        {
            try
            {
                var item = _context.Roles.FirstOrDefault(p => p.Id == role.Pvid);
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddRoleAccess(AppRoleAccess accessrights)
        {
            try
            {
                var exist = _context.IdentityRoleAccesses.FirstOrDefault(p => p.IdentityRolePvid == accessrights.IdentityRolePvid && p.IdentityModulePvid == accessrights.IdentityModulePvid && p.IdentityAccessLevelPvid == accessrights.IdentityAccessLevelPvid);
                if (exist != null && !exist.Pvid.Equals(0))
                    throw new Exception("Access right has already existed.");

                _context.IdentityRoleAccesses.Add(new IdentityRoleAccess()
                {
                    IdentityRolePvid = accessrights.IdentityRolePvid,
                    IdentityAccessLevelPvid = accessrights.IdentityAccessLevelPvid,
                    IdentityModulePvid = accessrights.IdentityModulePvid,
                    GrantedByIdentityUserPvid = accessrights.GrantedByIdentityUserPvid,
                    GrantedDate = accessrights.GrantedDate
                });

                _context.SaveChanges();
                await Task.CompletedTask;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteRoleAccess(AppRoleAccess accessrights)
        {
            try
            {
                var item = _context.IdentityRoleAccesses.FirstOrDefault(p => p.Pvid == accessrights.Pvid);
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Paging<AppRole>> GetRoles(DataTablePagingFilter filter)
        {
            var dto = new Paging<AppRole>();

            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await (from p in _context.Roles
                                   select new AppRole()
                                   {
                                       Pvid = p.Id,
                                       Name = p.Name,
                                       IsActive = p.isActive,
                                       EndActiveDate = p.EndActiveDate.Date,
                                       StartActiveDate = p.StartActiveDate.Date,
                                       AccessList = (from e in _context.IdentityRoleAccesses
                                                     where e.IdentityRolePvid == p.Id
                                                     select new AppRoleAccess()
                                                     {
                                                         IdentityRolePvid = p.Id,
                                                         IdentityAccessLevelPvid = e.IdentityAccessLevelPvid,
                                                         IdentityModulePvid = e.IdentityModulePvid,
                                                         Pvid = e.Pvid,
                                                         GrantedByIdentityUserPvid = e.GrantedByIdentityUserPvid,
                                                         GrantedDate = e.GrantedDate,
                                                         //GrantedBy = (from u in _context.Users
                                                         //             where u.Id == e.GrantedByIdentityUserPvid
                                                         //             select new AppUser()
                                                         //             {
                                                         //                 UserID = u.Id,
                                                         //                 Name = u.Name,
                                                         //                 UserName = u.UserName != null ? u.UserName : "",
                                                         //             }).FirstOrDefault(),
                                                         //AccessLevel = (from al in _context.IdentityAccessLevels
                                                         //               where al.Pvid == e.IdentityAccessLevelPvid
                                                         //               select new AppAccessLevel()
                                                         //               {
                                                         //                   Pvid = al.Pvid,
                                                         //                   Name = al.Name
                                                         //               }).ToList()
                                                     }).ToList()

                                   }).ToListAsync();
               

                

                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    items = items.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = items.Count();

                items = items.OrderBy(p => p.Name).Skip(start).Take(end).ToList();

                


                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

                return dto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            
        }

        public async Task<Paging<AppRoleAccess>> GetRoleAccessDetails(DataTablePagingRoleAccessFilter filter)
        {
            var dto = new Paging<AppRoleAccess>();

            try
            {
                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await(from p in _context.IdentityRoleAccesses
                                  where p.IdentityRolePvid == filter.RolePvid
                                  select new AppRoleAccess()
                                  {
                                      Pvid = p.Pvid,
                                      IdentityRolePvid = p.IdentityRolePvid,
                                      IdentityModulePvid = p.IdentityModulePvid,
                                      IdentityAccessLevelPvid = p.IdentityAccessLevelPvid,
                                      GrantedByIdentityUserPvid = p.GrantedByIdentityUserPvid,
                                      GrantedDate = p.GrantedDate.Date,
                                      GrantedBy = (from u in _context.Users
                                                   where u.Id == p.GrantedByIdentityUserPvid
                                                   select new AppUser()
                                                   {
                                                       UserID = u.Id,
                                                       Name = u.Name,
                                                       UserName = u.UserName
                                                   }).FirstOrDefault(),
                                      Module = (from im in _context.IdentityModules
                                                where im.Pvid == p.IdentityModulePvid
                                                select new AppModule()
                                                {
                                                    Pvid = im.Pvid,
                                                    Name = im.Name
                                                }).FirstOrDefault(),
                                      AccessLevel = (from al in _context.IdentityAccessLevels
                                                     where al.Pvid == p.IdentityAccessLevelPvid
                                                     select new AppAccessLevel()
                                                     {
                                                         Pvid = al.Pvid,
                                                         Name = al.Name
                                                     }).FirstOrDefault()

                                  }).ToListAsync();


                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    items = items.Where(p => p.Module.Name.ToUpper().Contains(filter.sSearch.ToUpper()) || p.AccessLevel.Name.ToUpper().Contains(filter.sSearch.ToUpper()) || p.GrantedBy.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = items.Count();

                items = items.Skip(start).Take(end).ToList();



                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

                return dto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region ACCESS LEVEL
        public async Task<bool> AddAccessLevel(AppAccessLevel access)
        {
            try
            {
                var exist = _context.IdentityAccessLevels.FirstOrDefault(p => p.Name == access.Name);
                if (exist != null && !exist.Pvid.Equals(0))
                    throw new Exception("Access level has already existed.");

                _context.IdentityAccessLevels.Add(new IdentityAccessLevel()
                {
                    Name = access.Name
                });

                _context.SaveChanges();
                await Task.CompletedTask;

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateAccessLevel(AppAccessLevel access)
        {
            try
            {
                var exist = _context.IdentityAccessLevels.FirstOrDefault(p=>p.Name == access.Name);
                if (exist != null && !exist.Pvid.Equals(0) && !exist.Pvid.Equals(access.Pvid))
                    throw new Exception("Access level has already existed.");

                var item = _context.IdentityAccessLevels.FirstOrDefault(p => p.Pvid == access.Pvid);
                if(item != null)
                {
                    item.Name = access.Name;

                    _context.Update(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAccessLevel(DeleteAccessLevelFilter filter)
        {
            try
            {
                var item = _context.IdentityAccessLevels.FirstOrDefault(p => p.Pvid == filter.AccessLevelPvid);
                if (item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Paging<AppAccessLevel>> GetAccessLevels(DataTablePagingFilter filter)
        {
            try
            {
                var dto = new Paging<AppAccessLevel>();

                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await (from p in _context.IdentityAccessLevels
                                   select new AppAccessLevel()
                                   {
                                      Pvid = p.Pvid,
                                      Name = p.Name,
                                      AccessDetails = (from d in _context.IdentityAccessLevelDetails
                                                       join m in _context.IdentityModules on d.IdentityModulePvid equals m.Pvid
                                                       where d.IdentityAccessLevelPvid == p.Pvid
                                                       select new AppAccessLevelDetail()
                                                       {
                                                           Pvid = d.Pvid,
                                                           IdentityAccessLevelPvid = d.IdentityAccessLevelPvid,
                                                           IdentityModulePvid = d.IdentityModulePvid,
                                                           PermissionKey = d.PermissionKey,
                                                           Module = new AppModule()
                                                           {
                                                               Pvid = m.Pvid,
                                                               Name = m.Name,
                                                               Permissions = (from o in _context.IdentityModuleOrganizes
                                                                              where o.IdentityModulePvid == m.Pvid
                                                                              select new AppModuleOrganize()
                                                                              {
                                                                                  Pvid = o.Pvid,
                                                                                  PermissionKey = o.PermissionKey,
                                                                                  IdentityModulePvid = o.IdentityModulePvid
                                                                              }).ToList()
                                                           }
                                                       }).ToList()

                                   }).ToListAsync();




                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    items = items.Where(p => p.Name.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = items.Count();

                items = items.OrderBy(p => p.Name).Skip(start).Take(end).ToList();




                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

                return dto;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddAccessLevelDetail(AppAccessLevelDetail detail)
        {
            try
            {
                _context.IdentityAccessLevelDetails.Add(new IdentityAccessLevelDetail()
                {
                    IdentityAccessLevelPvid = detail.IdentityAccessLevelPvid,
                    IdentityModulePvid = detail.IdentityModulePvid,
                    PermissionKey = detail.PermissionKey
                });
                _context.SaveChanges();
                await Task.CompletedTask;

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAccessLevelDetail(DeleteAccessLevelDetailFilter filter)
        {
            try
            {
                var item = _context.IdentityAccessLevelDetails.FirstOrDefault(p=>p.IdentityModulePvid == filter.IdentityModulePvid && p.IdentityAccessLevelPvid == filter.IdentityAccessLevelPvid && p.PermissionKey.Contains(filter.PermissionKey));
                if(item != null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    await Task.CompletedTask;
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Paging<AppAccessLevelDetail>> GetAccessLevelDetails(DataTablePagingAccessDetailsFilter filter)
        {
            try
            {
                var dto = new Paging<AppAccessLevelDetail>();

                var start = filter.iDisplayStart;
                var end = filter.iDisplayLength + start;

                var items = await (from p in _context.IdentityAccessLevelDetails
                                   join m in _context.IdentityModules on p.IdentityModulePvid equals m.Pvid
                                   where p.IdentityAccessLevelPvid == filter.AccessLevelPvid
                                   select new AppAccessLevelDetail()
                                   {
                                       Pvid = p.Pvid,
                                       PermissionKey = p.PermissionKey,
                                       IdentityAccessLevelPvid = p.IdentityAccessLevelPvid,
                                       IdentityModulePvid = p.IdentityModulePvid,
                                       Module = new AppModule()
                                       {
                                           Pvid = m.Pvid,
                                           Name = m.Name
                                       }
                                   }).ToListAsync();




                if (!string.IsNullOrWhiteSpace(filter.sSearch))
                {
                    items = items.Where(p => p.Module.Name.ToUpper().Contains(filter.sSearch.ToUpper()) || p.PermissionKey.ToUpper().Contains(filter.sSearch.ToUpper())).ToList();
                }

                var total = items.Count();

                items = items.OrderBy(p => p.Module.Name).Skip(start).Take(end).ToList();




                if (items != null)
                {
                    dto.List = items;
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = total
                    };
                    dto.PagingInfo = info;
                }

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<AppModuleOrganize> GetModuleOrganizes(ModuleIDFilter filter)
        {
            try
            {
                var list = (from p in _context.IdentityModuleOrganizes
                            where p.IdentityModulePvid == filter.ModulePvid
                            select new AppModuleOrganize()
                            {
                                PermissionKey = p.PermissionKey,
                                IdentityModulePvid = p.IdentityModulePvid,
                                Pvid = p.Pvid
                            }).ToList();

                return list;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetModulesInHQ(int referId, int cid)
        {
            try
            {
                var item = await _context.IdentityModules.FirstOrDefaultAsync(p => p.CID == cid && p.referID == referId.ToString());
                var id = 0;
                if (item != null)
                {
                    //id = item.Id;
                }
                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> AddRestaurants(AppAccessLevel access)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRestaurants(AppAccessLevel access)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRestaurants(DeleteAccessLevelFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRestaurant(AppModule restaurant)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateFirstPassword(int userId)
        {
            try
            {
                var usr = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);
                if(usr != null)
                {
                    usr.FirstPasswordReset = true;
                    _context.Update(usr);
                    _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<string>> GetPermissionDefault(int userId)
        {
            var list = await (from a in _context.IdentityRoleAccesses
                              join b in _context.Roles on a.IdentityRolePvid equals b.Id
                              join c in _context.IdentityModules on a.IdentityModulePvid equals c.Pvid
                              join d in _context.IdentityAccessLevelDetails on a.IdentityModulePvid equals d.IdentityModulePvid
                              join e in _context.IdentityRoleProfiles on b.Id equals e.IdentityRolePvid
                              join f in _context.Users on e.IdentityUserPvid equals f.Id
                              where f.Id == userId && d.IdentityAccessLevelPvid == d.IdentityAccessLevelPvid
                              select d.PermissionKey
                              ).Distinct().ToListAsync();

            return list;
        }
        #endregion
    }
}
