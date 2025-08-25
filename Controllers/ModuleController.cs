using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AdminPortalV8.Helpers;
using Microsoft.AspNetCore.Authorization;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Extensions;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;

namespace AdminPortalV8.Controllers
{

    [ExtendedAuthorize]
    public class ModuleController : Controller
    {
        private readonly IGeneral _general;

        public ModuleController(IGeneral general)
        {
           _general = general;
        }

        [ContentPermission(
              Key = AppModuleKeys.Module.ViewPrimaryKey,
              Title = AppModuleKeys.Module.ViewTitle,
              Desc = AppModuleKeys.Module.ViewDescription,
              StaticAuthorization = AppModuleKeys.Module.ViewStaticAuthorized
              )]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(
            Key = AppModuleKeys.Module.AddPrimaryKey,
            Title = AppModuleKeys.Module.AddTitle,
            Desc = AppModuleKeys.Module.AddDescription,
            AssociatedKey = AppModuleKeys.Module.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Module.AddStaticAuthorized)]
        public async Task<JsonInfoResult> Register(ModuleViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                var model = dto.Register;
                var module = new AppModule()
                {
                    Name = model.Name
                };

                await _general.AddModule(module);

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
        [ContentPermission(Key = AppModuleKeys.Module.EditPrimaryKey,
            Title = AppModuleKeys.Module.EditTitle,
            Desc = AppModuleKeys.Module.EditDescription,
            AssociatedKey = AppModuleKeys.Module.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Module.EditStaticAuthorized)]
        public async Task<JsonInfoResult> EditModule(ModuleViewModel dto)
        {
            var info = new JsonInfoResult();
            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                var model = dto.Manage;
                var module = new AppModule()
                {
                    Name = model.Name,
                    Pvid = model.ModulePvid
                };

                await _general.UpdateModule(module);
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Success = false;
                info.Exception = new Exception(ex.Message);
            }

            return info;
        }

        [HttpPost]
        [ContentPermission(Key = AppModuleKeys.Module.RemovePrimaryKey,
            Title = AppModuleKeys.Module.RemoveTitle,
            Desc = AppModuleKeys.Module.RemoveDescription,
            AssociatedKey = AppModuleKeys.Module.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Module.RemoveStaticAuthorized)]
        public JsonInfoResult RemoveModule(AppModule dto)
        {
            var info = new JsonInfoResult();
            try
            {
                //_authService.DeleteModule(dto); 
                _general.DeleteModule(dto);
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            return info;
        }

        [ContentPermission(Key = AppModuleKeys.Module.ManagePrimaryKey,
            Title = AppModuleKeys.Module.ManageTitle,
            Desc = AppModuleKeys.Module.ManageDescription,
            AssociatedKey = AppModuleKeys.Module.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Module.ManageStaticAuthorized)]
        public void ManageModule() { }

        [HttpPost]
        [ContentPermission(Key = AppModuleKeys.Module.ManagePermissionPrimaryKey,
            Title = AppModuleKeys.Module.ManagePermissionTitle,
            Desc = AppModuleKeys.Module.ManagePermissionDescription,
            AssociatedKey = AppModuleKeys.Module.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.Module.ManagePermissionStaticAuthorized)]
        public JsonInfoResult ManagePermission(ManageModulePermissionViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (dto.Included)
                {
                    var dtotemp1 = new AppModuleOrganize
                    {
                        IdentityModulePvid = dto.ModulePvid,
                        PermissionKey = dto.PermissionKey
                    };

                    //2
                    //var content = AuthManager.PermissionManager.GetContent(dto.PermissionKey);
                    var contents = _general.GetAllContentPermissions();
                    var content = contents.FirstOrDefault(p => p.Key == dto.PermissionKey);
                    _general.AddModuleOrganize(dtotemp1);
                    //_authService.AddModuleOrganize(dtotemp1, content, AuthManager.PermissionManager.GetChildContents(dto.PermissionKey));
                }
                else
                {
                    var dtotemp2 = new DeleteModuleOrganizeFilter
                    {
                        IdentityModulePvid = dto.ModulePvid,
                        PermissionKey = dto.PermissionKey
                    };
                    //_authService.DeleteModuleOrganize(dtotemp2); 3
                    _general.DeleteModuleOrganize(dtotemp2);
                }

                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Success = false;
                info.Exception = new Exception(ex.Message);
            }

            return info;
        }


        [HttpPost]
        public async Task<JsonResult> GetModuleOrganize(DataTablePagingModuleOrganizeFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {
                
                var dto = await _general.GetModulesOrganize(filter);
                return DataTableSerializer.SerializeDataTable<AppModuleOrganize>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return Json(result);
        }

        [HttpGet]
        //[ContentPermission(ModuleKey.Module.GetModules,AssociatedKey = ModuleKey.Module.View,
        //    Title="View Modules",Desc = "Display all Available Modules"
        //    ,StaticAuthorization = true)]
        public async Task<JsonResult> GetModules(DataTablePagingFilter filter)
        {
            var info = new JsonInfoResult();
            try
            {
                var dto = await _general.GetModules(filter);
                return DataTableSerializer.SerializeDataTable<AppModule>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }

            
            return Json(info);
        }
    }
}
