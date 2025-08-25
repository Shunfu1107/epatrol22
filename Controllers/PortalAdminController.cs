using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using AdminPortalV8.Data.ExtendedIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Extensions;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Authorization;
using System.Web.WebPages.Html;
using System.Security.Claims;
using System.Reflection;
using static AdminPortalV8.Helpers.AppModuleKeys;
using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Models.User;
using AdminPortalV8.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OAuth;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using AdminPortalV8.Models;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class PortalAdminController : Controller
    {
        private RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeneral _general;
        private readonly IUserService _userService;
        private readonly IUser _user;
        private readonly IDashboardService _dashboard;
        private readonly IRestaurant _restaurant;
        private readonly IAnalytic _analytic;

        public PortalAdminController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager,
            IGeneral general, IUser user, IDashboardService dashboard, IRestaurant restaurant, IAnalytic analytic)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _general = general;
            _user = user;
            _dashboard = dashboard;
            _restaurant = restaurant;
            _analytic = analytic;
        }

        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.ViewTitle,
            Desc = AppModuleKeys.PortalAdmin.ViewDescription,
            StaticAuthorization = AppModuleKeys.PortalAdmin.ViewStaticAuthorized
            )]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Where(p => p.isActive == true).ToListAsync();
            var user = HttpContext.User;
            var systemAuthorized = user.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.SystemPrimaryKey);
            var adminAuthorized = user.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.AdminPrimaryKey);
            if (!systemAuthorized)
            {
                var systemrole = roles.FirstOrDefault(r => r.Name.ToUpper() == "SYSTEM ADMIN");
                roles.Remove(systemrole);
            }
            if (!adminAuthorized)
            {
                var adminrole = roles.FirstOrDefault(r => r.Name.ToUpper() == "ADMIN");
                roles.Remove(adminrole);
            }
            var adduserrole = new AddUserRoleViewModel();

            adduserrole.Roles = roles;

            var addExclusive = new AddExclusiveAccessViewModel();


            var objDenied = new { Value = false, Title = "Denied" };
            var objGranted = new { Value = true, Title = "Granted" };
            var list = new List<object>();
            list.Add(objGranted);
            list.Add(objDenied);
            addExclusive.Accessibles = list;

            var editExclusive = new EditExclusiveAccessViewModel();
            editExclusive.Accessibles = list;

            var modal = new PortalAdminViewModel();
            modal.AddExclusiveAccess = addExclusive;
            modal.AddUserRole = adduserrole;
            modal.EditExclusiveAccess = editExclusive;

            var rest = new AddUserRestaurant();
            var lists = await _restaurant.GetListRestaurant();
            rest.RestaurantList = lists.Where(p => p.Active == "Yes").ToList();

            modal.AddUserRestaurant = rest;


            return View(modal);
        }

        public IList<AppModuleOrganize> FullPermission(IList<AppModuleOrganize> model)
        {
            var newModel = new List<AppModuleOrganize>();

            var ModuleList = new List<string>();

            // Retrieve data from AppModuleKeys
            Type appModuleKeysType = typeof(AppModuleKeys);
            var nestedClasses = appModuleKeysType.GetNestedTypes(BindingFlags.Public);

            foreach (var nestedClass in nestedClasses)
            {
                var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var field in fields)
                {
                    var fieldInfo = field.Name;
                    if(fieldInfo.Contains("Key"))
                    {
                        ModuleList.Add(field.GetValue(null).ToString());
                    }
                    
                }
            }


            try
            {
                if(model.Count > 0)
                {
                    foreach(var item in model)
                    {
                        var perm = item.PermissionKey.Split(".");

                        var list = ModuleList.Where(p => p.Split(".")[0] == perm[0]).ToList();

                        foreach(var l in list)
                        {
                            if(newModel.FirstOrDefault(p=>p.PermissionKey == l) == null)
                            {
                                newModel.Add(new AppModuleOrganize()
                                {
                                    PermissionKey = l,
                                    Pvid = item.Pvid,
                                    IdentityModulePvid = item.IdentityModulePvid
                                });
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }

            model = newModel;

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.AddExclusiveAccessRightsPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.AddExclusiveAccessRightsTitle,
            Desc = AppModuleKeys.PortalAdmin.AddExclusiveAccessRightsDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.AddExclusiveAccessRightsStaticAuthorized)]
        public JsonInfoResult AddAccessExclusive(PortalAdminViewModel model)
        {
            var result = new JsonInfoResult();
            try
            {
                _general.AddExclusiveAccess(model.AddExclusiveAccess);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = new Exception(ex.Message);
            }

            return result;
        }

        [HttpGet]
        public JsonResult GetExclusiveDetail(int exclusiveID)
        {
            try
            {
                var item = _general.GetAccessExclusiveByID(exclusiveID);

                var status = true;
                var data = item;
                return Json(new { status, data });
            }
            catch(Exception ex )
            {
                var status = false;
                var data = ex.Message;
                return Json(new { status, data });
            }
        }

        [HttpGet]
        public JsonResult GetModuleWithPermission()
        {
            try
            {
                var item = new List<AppModule>();

                item = _general.GetModuleWithPermissionList();

                foreach(var mod in item)
                {
                    mod.Permissions = FullPermission(mod.Permissions);
                }

                var status = true;
                var data = item;
                return Json(new { status, data });
            }
            catch(Exception ex)
            {
                var status = false;
                var data = ex.Message;
                return Json(new { status, data });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUsers(DataTablePagingFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {

                //var dto = _authService.GetUsers(filter);
                var dto = await _general.GetAllUsers(filter);
                return DataTableSerializer.SerializeDataTable(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
                
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.AddRolePrimaryKey,
            Title = AppModuleKeys.PortalAdmin.AddRoleTitle,
            Desc = AppModuleKeys.PortalAdmin.AddRoleDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.AddRoleStaticAuthorized)]
        public async Task<JsonInfoResult> AddRole(PortalAdminViewModel modal)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                //var service = new AuthService();
                //var claimid = AuthManager.Identity.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
                var claimid = HttpContext.User.Claims.FirstOrDefault(m=>m.Type == ClaimTypes.NameIdentifier);
                var user = default(ApplicationUser);

                if (claimid != null)
                    user = await _userManager.FindByIdAsync(claimid.Value);

               // await _volunteer.VerifyVolunteerAsync(Convert.ToInt32(modal.AddUserRole.UserPvid), user.Name, Convert.ToInt32(modal.AddUserRole.RolePvid));

                //await _apiService.VerifyVolunteerAsync(new Models.Volunteer.VerifyVolunteerViewModel()
                //{
                //    UserId = Convert.ToInt32(modal.AddUserRole.UserPvid),
                //    RoleId = Convert.ToInt32(modal.AddUserRole.RolePvid),
                //    VerifiedBy = user.Name
                //});

                //_authService.AddUserRole(modal.AddUserRole);
                await _general.AddUserRoles(modal.AddUserRole);
                
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Exception = new Exception(ex.Message);
                info.Success = false;
            }

            return info;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.AddRestaurantPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.AddRestaurantTitle,
            Desc = AppModuleKeys.PortalAdmin.AddRestaurantDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.AddRestaurantStaticAuthorized)]
        public async Task<JsonInfoResult> AddRestaurant(PortalAdminViewModel modal)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                var result = await _restaurant.AddUserRestaurant(modal.AddUserRestaurant);

                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Exception = new Exception(ex.Message);
                info.Success = false;
            }

            return info;
        }

        [HttpGet]
        public JsonResult CheckExistingExclusive(int userID, string key, int pvid)
        {
            try
            {
                

                var exist = _general.CheckExistingExclusive(userID, key, pvid);

                var status = true;
                var data = exist;
                return Json(new { status, data });
            }
            catch (Exception ex)
            {
                var status = false;
                var data = ex.Message;
                return Json(new { status, data });
            }
        }

        [HttpGet]
        public async Task<JsonResult> CheckExistingRole(int roleID, int userID)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userID.ToString());
                var role = await _roleManager.FindByIdAsync(roleID.ToString());

                var exist = _general.CheckExistingUserRole(user, role);

                var status = true;
                var data = exist;
                return Json(new { status, data });
            }
            catch(Exception ex)
            {
                var status = false;
                var data = ex.Message;
                return Json(new { status, data });
            }
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.RemoveRolePrimaryKey,
            Title = AppModuleKeys.PortalAdmin.RemoveRoleTitle,
            Desc = AppModuleKeys.PortalAdmin.RemoveRoleDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.RemoveRoleStaticAuthorized)]
        public async Task<JsonInfoResult> RemoveRole(DeleteRoleProfileFilter filter)
        {
            var info = new JsonInfoResult();
            try
            {
                await _general.RemoveRoleUserProfiles(Convert.ToInt32(filter.RolePvid), Convert.ToInt32(filter.UserPvid));
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            return info;
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.RemoveRestaurantPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.RemoveRestaurantTitle,
            Desc = AppModuleKeys.PortalAdmin.RemoveRestaurantDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.RemoveRestaurantStaticAuthorized)]
        public async Task<JsonInfoResult> RemoveRestaurant(DeleteRestaurantProfileFilter filter)
        {
            var info = new JsonInfoResult();
            try
            {
                var result = await _restaurant.RemoveUserRestaurant(Convert.ToInt32(filter.RestPvid));
                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            return info;
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.RemoveExclusiveAccessRightsPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.RemoveExclusiveAccessRightsTitle,
            Desc = AppModuleKeys.PortalAdmin.RemoveExclusiveAccessRightsDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.RemoveExclusiveAccessRightsStaticAuthorized)]
        public async Task<JsonInfoResult> RemoveAccessExclusive(AppAccessLevelExclusive exclusive)
        {
            var result = new JsonInfoResult();
            try
            {
                await _general.RemoveAccessExclusive(Convert.ToInt32(exclusive.Pvid));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = new Exception(ex.Message);
            }

            return result;
        }

        [HttpGet]
        public async Task<JsonResult> GetRoleByID(int id)
        {
            try
            {
                var roles = await _roleManager.FindByIdAsync(id.ToString());
                var status = true;
                var data = roles.Name;

                return Json(new { status, data });
            }
            catch(Exception ex)
            {
                var status = false;
                var data = ex.Message;
                return Json(new { status, data });
            }
        }

        [HttpGet]
        public JsonResult GetUserByID(int userID)
        {
            try
            {
                var result = _general.GetUserByID(userID);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.RemovePrimaryKey,
            Title = AppModuleKeys.PortalAdmin.RemoveTitle,
            Desc = AppModuleKeys.PortalAdmin.RemoveDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.RemoveStaticAuthorized)]
        public async Task<JsonInfoResult> RemoveUser(int userID)
        {
            var result = new JsonInfoResult();
            try
            {
                var user = await _userManager.FindByIdAsync(userID.ToString());
                if (user != null)
                {
                    bool remove = _general.DeleteUserPerson(userID);

                    if(remove)
                    {
                        var asyncresult = await _userManager.DeleteAsync(user);
                        if (!asyncresult.Succeeded) return result.ThrowAsyncError(asyncresult);
                        result.Success = true;
                    }
                }
                

                
                
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = new Exception(ex.Message);
            }

            return result;
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.ManageAccessRightsPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.ManageAccessRightsTitle,
            Desc = AppModuleKeys.PortalAdmin.ManageAccessRightsDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.ManageAccessRightsStaticAuthorized)]
        public async Task<JsonResult> GetUserRoles(DataTablePagingUserRolesFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {
                var dto = await _general.GetUserRoles(Convert.ToInt32(filter.UserPvid));
                return DataTableSerializer.SerializeDataTable<AppRole>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetRestaurant(DataTablePagingUserRolesFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {
                var dto = await _dashboard.GetUserRestaurant(Convert.ToInt32(filter.UserPvid));
                return DataTableSerializer.SerializeDataTable<UserRestaurantModel>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetExclusiveAccess(DataTablePagingUserRolesFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {
                var dto = await _general.GetUserAccessExclusive(Convert.ToInt32(filter.UserPvid));
                return DataTableSerializer.SerializeDataTable<AppAccessLevelExclusive>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
            }

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
           Key = AppModuleKeys.PortalAdmin.AddPrimaryKey,
           Title = AppModuleKeys.PortalAdmin.AddTitle,
           Desc = AppModuleKeys.PortalAdmin.AddDescription,
           AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
           StaticAuthorization = AppModuleKeys.PortalAdmin.EditStaticAuthorized)]
        public async Task<JsonInfoResult> AddUser(PortalAdminViewModel wrapper)
        {
            var result = new JsonInfoResult();
            try
            {
                var model = wrapper.RegisterViewModel;
                if (!ModelState.IsValid)
                {
                    throw new ApplicationException("Please fill in required field");
                }

                if (ModelState.IsValid)
                {
                    var userIsExist = _general.GetUserByEmail(model.Email);
                    if (userIsExist == null)
                    {
                        var user = new User()
                        {
                            Name = model.Name,
                            Email = model.Email,
                            UserName = model.UserName,
                            ReceivedEmail = model.ReceivedEmail

                        };
                        PortalAdminViewModel portal = new PortalAdminViewModel()
                        {
                            RegisterViewModel = wrapper.RegisterViewModel
                        };

                        // Create new User in User table
                        //var register = await _portalAdminController.Register(portal);
                        var register = await _userManager.CreateAsync(new ApplicationUser()
                        {
                            UserName = portal.RegisterViewModel.UserName,
                            NormalizedUserName = portal.RegisterViewModel.UserName.ToUpper(),
                            Name = portal.RegisterViewModel.Name,
                            Email = portal.RegisterViewModel.Email,
                            NormalizedEmail = portal.RegisterViewModel.Email.ToUpper(),
                            EmailConfirmed = false,
                            Active = true,
                            PhoneNumberConfirmed = false,
                            TwoFactorEnabled = false,
                            LockoutEnabled = true,
                            AccessFailedCount = 0,
                            //Password = portal.RegisterViewModel.Password,
                            FirstPasswordReset = false,
                            cid = 0,
                            referId = string.Empty,
                            ReceivedEmail = portal.RegisterViewModel.ReceivedEmail,
                        }, portal.RegisterViewModel.Password);

                        
                        
                        if (!register.Succeeded) return result.ThrowAsyncError(register);
                        bool updateSuccess = _general.UpdateReceivedEmail(portal.RegisterViewModel.UserName, portal.RegisterViewModel.ReceivedEmail);
                        result.Success = true;
                        result.Data = new { available = true };
                    }
                    else
                    {
                        result.Success = true;
                        result.Data = new { available = false };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Success = false;
            }
            return result;
        }

            [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
           Key = AppModuleKeys.PortalAdmin.EditPrimaryKey,
           Title = AppModuleKeys.PortalAdmin.EditTitle,
           Desc = AppModuleKeys.PortalAdmin.EditDescription,
           AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
           StaticAuthorization = AppModuleKeys.PortalAdmin.EditStaticAuthorized)]
        public async Task<JsonInfoResult> EditUser(PortalAdminViewModel wrapper)
        {
            var result = new JsonInfoResult();
            //UserPasswordHistory PassDB = new UserPasswordHistory();


            try
            {
                if (!ModelState.IsValid) return result.ThrowModelStateError(ModelState);

                var model = wrapper.ManageUserViewModel;
                if (ModelState.IsValid)
                {

                    var selectedUser = await _userManager.FindByIdAsync(model.UserID.ToString());





                    UserPasswordHistory PassHistory = _general.GetUserPasswordHistory(Convert.ToInt32(selectedUser.Id));

                    if (PassHistory == null)
                    {
                        UserPasswordHistory userPass = new UserPasswordHistory();
                        userPass.UserID = Convert.ToInt32(selectedUser.Id);
                        userPass.PasswordHash1 = selectedUser.PasswordHash;
                        userPass.PasswordDate1 = DateTime.Now;
                        _general.UpdatePasswordHistory(userPass);

                        var verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, selectedUser.Password, model.Password);
                        if (verify.ToString().Contains("Success"))
                        {
                            result.Success = true;
                            result.Data = new { stat = true, check = true };
                            return result;
                        }
                    }
                    else
                    {
                        bool check = false;
                        var passwordhash = _userManager.PasswordHasher.HashPassword(selectedUser, model.Password);
                        var verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash1, model.Password);
                        if (verify.ToString().Contains("Success"))
                        {
                            check = true;
                        }
                        else
                        {
                            verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash2, model.Password);
                            if (verify.ToString().Contains("Success"))
                            {
                                check = true;
                            }
                            else
                            {
                                verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash3, model.Password);
                                if (verify.ToString().Contains("Success"))
                                {
                                    check = true;
                                }
                                else
                                {
                                    verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash4, model.Password);
                                    if (verify.ToString().Contains("Success"))
                                    {
                                        check = true;
                                    }
                                    else
                                    {
                                        verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash5, model.Password);
                                        if (verify.ToString().Contains("Success"))
                                        {
                                            check = true;
                                        }
                                        else
                                        {
                                            verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash6, model.Password);
                                            if (verify.ToString().Contains("Success"))
                                            {
                                                check = true;
                                            }
                                            else
                                            {
                                                verify = _userManager.PasswordHasher.VerifyHashedPassword(selectedUser, PassHistory.PasswordHash7, model.Password);
                                                if (verify.ToString().Contains("Success"))
                                                {
                                                    check = true;
                                                }
                                                else
                                                {
                                                    check = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        if (check == true)
                        {
                            result.Success = true;
                            result.Data = new { stat = true, check = true };
                            return result;
                        }
                    }

                    if (selectedUser.Password != model.Password)
                    {
                        var asyncresult1 = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, selectedUser, model.Password);
                        if (!asyncresult1.Succeeded) return result.ThrowAsyncError(asyncresult1);
                        var passwordhash = _userManager.PasswordHasher.HashPassword(selectedUser, model.Password);
                        var newsecurityStamp = Guid.NewGuid().ToString();

                        var deletPass = await _userManager.RemovePasswordAsync(selectedUser);

                        var a = await _userManager.AddPasswordAsync(selectedUser, passwordhash);
                        selectedUser.SecurityStamp = newsecurityStamp;
                        var b = await _userManager.UpdateSecurityStampAsync(selectedUser);
                        selectedUser.Password = passwordhash;
                        selectedUser.PasswordHash = passwordhash;
                        var c = await _userManager.UpdateAsync(selectedUser);

                        //await store.SetPasswordHashAsync(selectedUser, passwordhash);
                        //await store.SetSecurityStampAsync(selectedUser, newsecurityStamp);
                        //await store.UpdateAsync(selectedUser);
                        selectedUser = await _userManager.FindByIdAsync(model.UserID.ToString());
                    }



                    selectedUser.Name = model.Name;
                    selectedUser.Email = model.Email;
                    selectedUser.UserName = model.UserName;
                    selectedUser.Active = model.Active;

                    var results = await _userManager.UpdateAsync(selectedUser);

                    if (results.Succeeded)
                    {
                        var updatePerson = await _general.UpdateNameForPersonByUserIdAsync(
                        new UpdateNameForPersonViewModel()
                        { UserId = Convert.ToInt32(selectedUser.Id), Name = selectedUser.Name });

                        if (!updatePerson)
                        {
                            throw new Exception("Update Person's name failed.");
                        }

                        bool updateSuccess = _general.UpdateReceivedEmail(selectedUser.UserName, model.ReceivedEmail);

                    }



                    UserPasswordHistory userP = new UserPasswordHistory();
                    userP.UserID = Convert.ToInt32(selectedUser.Id);
                    userP.PasswordHash1 = selectedUser.Password;
                    userP.PasswordDate1 = DateTime.Now;
                    _general.UpdatePasswordHistory(userP);



                }
                result.Data = new { stat = true, check = false };
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = new Exception(ex.Message);
                result.Success = false;
                throw new Exception("Try Again");
            }

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
            Key = AppModuleKeys.PortalAdmin.EditExclusiveAccessRightsPrimaryKey,
            Title = AppModuleKeys.PortalAdmin.EditExclusiveAccessRightsTitle,
            Desc = AppModuleKeys.PortalAdmin.EditExclusiveAccessRightsDescription,
            AssociatedKey = AppModuleKeys.PortalAdmin.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.PortalAdmin.EditExclusiveAccessRightsStaticAuthorized)]
        public async Task<JsonInfoResult> EditAccessExclusiveAsync(PortalAdminViewModel model)
        {
            var result = new JsonInfoResult();
            try
            {
                //_authService.UpdateAccessExclusive(model.EditExclusiveAccess);
                var results = await _general.UpdateExclusiveAccess(model.EditExclusiveAccess);

                if(results)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                }
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = new Exception(ex.Message);
            }

            return result;
        }

       


    }
}
