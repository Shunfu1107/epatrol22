using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Extensions;
using static AdminPortalV8.Helpers.AppModuleKeys;
using Microsoft.AspNetCore.Identity;
using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class RoleController : Controller
    {
        private readonly IGeneral _general;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleController(IGeneral general, RoleManager<ApplicationRole> roleManager)
        {
            _general = general;
            _roleManager = roleManager;
        }
        [ContentPermission(
         Key = AppModuleKeys.Role.ViewPrimaryKey,
         Title = AppModuleKeys.Role.ViewTitle,
         Desc = AppModuleKeys.Role.ViewDescription,
         StaticAuthorization = AppModuleKeys.Role.ViewStaticAuthorized
         )]
        public ActionResult Index()
        {
            var modules = _general.GetAllActiveModule();
            var accessLevels = _general.GetAllAccessLevel();
            var temp = new AddAccessRightViewModel();
            temp.AccessLevelList = accessLevels;
            temp.ModuleList = modules;


            var model = new RoleViewModel();
            model.AddAccessRight = temp;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(Key = AppModuleKeys.Role.AddPrimaryKey,
            Title = AppModuleKeys.Role.AddTitle,
            Desc = AppModuleKeys.Role.AddDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.AddStaticAuthorized)]
        public async Task<JsonInfoResult> Register(RoleViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);
                var result = await _general.AddRole(dto.Register);

                info.Success = result;
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
        [ContentPermission(Key = AppModuleKeys.Role.EditPrimaryKey,
            Title = AppModuleKeys.Role.EditTitle,
            Desc = AppModuleKeys.Role.EditDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.EditStaticAuthorized)]
        public async Task<JsonInfoResult> UpdateRole(RoleViewModel dto)
        {
            var info = new JsonInfoResult();
            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);
                var result = await _general.UpdateRole(dto.Manage);
                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Success = false;
                info.Exception = new Exception(ex.Message);
            }

            return info;
        }

        [HttpPost]
        [ContentPermission(Key = AppModuleKeys.Role.RemovePrimaryKey,
            Title = AppModuleKeys.Role.RemoveTitle,
            Desc = AppModuleKeys.Role.RemoveDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.RemoveStaticAuthorized)]
        public JsonInfoResult RemoveRole(AppRole dto)
        {
            var info = new JsonInfoResult();
            try
            {
                var result = _general.DeleteRole(dto);
                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            return info;
        }

        [ContentPermission(Key = AppModuleKeys.Role.ManagePrimaryKey,
            Title = AppModuleKeys.Role.ManageTitle,
            Desc = AppModuleKeys.Role.ManageDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.ManageStaticAuthorized)]
        public void ManageRole() { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(Key = AppModuleKeys.Role.AddRoleAccessPrimaryKey,
            Title = AppModuleKeys.Role.AddRoleAccessTitle,
            Desc = AppModuleKeys.Role.AddRoleAccessDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.AddRoleAccessStaticAuthorized)]
        public async Task<JsonInfoResult> AddAccessRight(RoleViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);
                var result = await _general.AddRoleAccess(dto.AddAccessRight);
                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Exception = new Exception(ex.Message);
                info.Success = false;
            }

            return info;
        }

        [HttpPost]
        [ContentPermission(Key = AppModuleKeys.Role.RemoveRoleAccessPrimaryKey,
            Title = AppModuleKeys.Role.RemoveRoleAccessTitle,
            Desc = AppModuleKeys.Role.RemoveRoleAccessDescription,
            AssociatedKey = AppModuleKeys.Role.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Role.RemoveRoleAccessStaticAuthorized)]
        public JsonInfoResult RemoveAccessRight(AppRoleAccess dto)
        {
            var info = new JsonInfoResult();
            try
            {
                var result = _general.DeleteRoleAccess(dto);
                info.Success = result;
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            return info;
        }


        public async Task<JsonResult> GetRoles(DataTablePagingFilter filter)
        {
            var info = new JsonInfoResult();

            try
            {
                var dto = await _general.GetRoles(filter);
                return DataTableSerializer.SerializeDataTable<AppRole>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            
            return Json(info);
        }

        [HttpGet]
        public async Task<JsonResult> CheckRoleNameExist(RoleViewModel model)
        {
            var name = model.Register.Name;
            var role = await _roleManager.FindByNameAsync(name);
            if (role != null && !role.Id.Equals(0)) return Json(false);
            return Json(true);
        }

        [HttpPost]
        public async Task<JsonResult> GetRoleAccessDetails(DataTablePagingRoleAccessFilter filter)
        {
            var info = new JsonInfoResult();

            try
            {
                var dto = await _general.GetRoleAccessDetails(filter);
                return DataTableSerializer.SerializeDataTable<AppRoleAccess>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }


            return Json(info);
        }

        [HttpPost]
        public JsonInfoResult GetAllActiveModule()
        {
            var info = new JsonInfoResult();

            try
            {
                var dto = _general.GetAllActiveModule();
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Exception = new Exception(ex.Message);
                info.Success = false;
            }

            return info;
        }


    }
}
