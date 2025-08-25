using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Extensions;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Services;
using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class AccessLevelController : Controller
    {
        private readonly IGeneral _general;

        public AccessLevelController(IGeneral general)
        {
            _general = general;
        }

        [ContentPermission(
             Key = AppModuleKeys.AccessLevel.ViewPrimaryKey,
             Title = AppModuleKeys.AccessLevel.ViewTitle,
             Desc = AppModuleKeys.AccessLevel.ViewDescription,
             StaticAuthorization = AppModuleKeys.AccessLevel.ViewStaticAuthorized
             )]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ContentPermission(Key = AppModuleKeys.AccessLevel.AddPrimaryKey,
            Title = AppModuleKeys.AccessLevel.AddTitle,
            Desc = AppModuleKeys.AccessLevel.AddDescription,
            AssociatedKey = AppModuleKeys.AccessLevel.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.AccessLevel.AddStaticAuthorized)]
        public async Task<JsonInfoResult> Register(AccessLevelViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                var model = dto.Register;
                var access = new AppAccessLevel()
                {
                    Name = model.Name
                };
                //_authService.AddAccessLevel(access);
                bool result = await _general.AddAccessLevel(access);


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
        [ContentPermission(Key = AppModuleKeys.AccessLevel.EditPrimaryKey,
            Title = AppModuleKeys.AccessLevel.EditTitle,
            Desc = AppModuleKeys.AccessLevel.EditDescription,
            AssociatedKey = AppModuleKeys.AccessLevel.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.AccessLevel.EditStaticAuthorized)]
        public async Task<JsonInfoResult> UpdateAccessLevel(AccessLevelViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                if (!ModelState.IsValid) return info.ThrowModelStateError(ModelState);

                var model = dto.Manage;
                var access = new AppAccessLevel()
                {
                    Pvid = model.AccessLevelPvid,
                    Name = model.Name
                };
                //_authService.UpdateAccessLevel(access);

                var result = await _general.UpdateAccessLevel(access);

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
        [ContentPermission(Key = AppModuleKeys.AccessLevel.RemovePrimaryKey,
            Title = AppModuleKeys.AccessLevel.RemoveTitle,
            Desc = AppModuleKeys.AccessLevel.RemoveDescription,
            AssociatedKey = AppModuleKeys.AccessLevel.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.AccessLevel.RemoveStaticAuthorized)]
        public async Task<JsonInfoResult> RemoveAccessLevel(AppAccessLevel dto)
        {
            var info = new JsonInfoResult();
            try
            {
                //_authService.DeleteAccessLevel(dto);
                var result = await _general.DeleteAccessLevel(dto);
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
        [ContentPermission(Key = AppModuleKeys.AccessLevel.ManagePrimaryKey,
            Title = AppModuleKeys.AccessLevel.ManageTitle,
            Desc = AppModuleKeys.AccessLevel.ManageDescription,
            AssociatedKey = AppModuleKeys.AccessLevel.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.AccessLevel.ManageStaticAuthorized)]
        public void ManageAccess(AccessLevelViewModel dto) { }

        [HttpPost]
        public async Task<JsonResult> GetAccessLevels(DataTablePagingFilter filter)
        {
            var info = new JsonInfoResult();

            try
            {
                //var dto = _authService.GetAccessLevels(filter);
                var dto = await _general.GetAccessLevels(filter);
                return DataTableSerializer.SerializeDataTable<AppAccessLevel>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                info.Exception = ex;
                info.Success = false;
            }


            return Json(info);
        }

        [HttpPost]
        [ContentPermission(Key = AppModuleKeys.AccessLevel.ManagePermissionPrimaryKey,
            Title = AppModuleKeys.AccessLevel.ManagePermissionTitle,
            Desc = AppModuleKeys.AccessLevel.ManagePermissionDescription,
            AssociatedKey = AppModuleKeys.AccessLevel.ViewPrimaryKey,
            StaticAuthorization = AppModuleKeys.AccessLevel.ManagePermissionStaticAuthorized)]
        public async Task<JsonInfoResult> ManagePermission(ManageAccessLevelPermissionViewModel dto)
        {
            var info = new JsonInfoResult();

            try
            {
                var result = false;

                if (dto.Included) result = await _general.AddAccessLevelDetail(dto);//_authService.AddAccessLevelDetail(dto);
                else result = await _general.DeleteAccessLevelDetail(dto); //_authService.DeleteAccessLevelDetail(dto);

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
        public async Task<JsonResult> GetAccessDetails(DataTablePagingAccessDetailsFilter filter)
        {
            var result = new JsonInfoResult();

            try
            {
                var dto = await _general.GetAccessLevelDetails(filter);//_authService.GetAccessLevelDetails(filter);
                return DataTableSerializer.SerializeDataTable<AppAccessLevelDetail>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return Json(result);
        }
        [HttpPost]
        public JsonInfoResult GetAllActiveModule()
        {
            var info = new JsonInfoResult();

            try
            {
                var dto = _general.GetAllActiveModule();//_authService.GetAllActiveModule();
                info.Data = dto;
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
        public JsonInfoResult GetModuleOrganize(ModuleIDFilter filter)
        {
            var info = new JsonInfoResult();
            try
            {
                var dto = _general.GetModuleOrganizes(filter);//_authService.GetModuleOrganize(filter);
                info.Data = dto;
                info.Success = true;
            }
            catch (Exception ex)
            {
                info.Success = false;
                info.Exception = new Exception(ex.Message);
            }

            return info;
        }

       
    }
}
