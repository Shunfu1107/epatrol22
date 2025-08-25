using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Models.User;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Security.Claims;   


namespace AdminPortalV8.Controllers
{
    public class AccountController : Controller                             
    {
        private readonly IAuth _authService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;
        private readonly IRestaurant _restaurant;

        public AccountController(IAuth authService, UserManager<ApplicationUser> manager, UserObj usrObj, IGeneral general, IRestaurant restaurant)
        {
            _authService = authService;
            userManager = manager;
            _usrObj = usrObj;
            _general = general;
            _restaurant = restaurant;
        }

        [HttpPost]
        public JsonInfoResult GetConfig()
        {
            var result = new JsonInfoResult();
            string configJson = HttpContext.Session.GetString("GetConfig");

            

            if (!string.IsNullOrEmpty(configJson))
            {
                configJson = configJson.Trim();

          
                try
                {
                    var tempData = JsonConvert.DeserializeObject<JsonInfoResult>(configJson);
                    var data = tempData.Data.ToString();
                    var configModel = JsonConvert.DeserializeObject<ConfigModel>(data);
                   
                    result.Success = tempData.Success;
                    result.Data = configModel;
                }
                catch (JsonReaderException ex)
                {
                    result.Exception = new Exception(ex.Message);
                    result.Success = false;
                }
            }
            else
            {
                try
                {
                    var claimid = HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
                    var userprofile = default(AppUserProfiles);
                    var User = default(AppUser);


                    if (claimid == null) goto LB_ANONYMOUSE;
                    userprofile = _authService.GetUserProfiles(claimid.Value);
                    User = _authService.GetUserByID(long.Parse(claimid.Value));

                LB_ANONYMOUSE:
                    result.Data = new
                    {
                        User = User,
                        UserProfiles = userprofile,
                        Permission = _general.GetAllContentPermissions()
                        

                };
                    result.Success = true;
                    var json = JsonConvert.SerializeObject(result);
                    HttpContext.Session.SetString("GetConfig", json);
                    _usrObj.user = User;
                    _usrObj.userProfiles = userprofile;
                  

                }
                catch (Exception ex)
                {
                    result.Exception = new Exception(ex.Message);
                    result.Success = false;
                }
            }

            

            

            return result;
        }

        [HttpGet]
        public async Task<JsonInfoResult> Restaurant()
        {
            var result = new JsonInfoResult();

            var list = new List<SelectListItem>();
            

            var items = await _restaurant.GetUserRestaurantList(Convert.ToInt32(_usrObj.user.Id));
            var all = false;
            if(_usrObj.RestaurantId == 0)
            {
                all = true;
            }
            if(items.Count > 0)
            {
                list.Add(new SelectListItem() { Value = "0", Text = "All restaurants", Selected = all });
            }
            foreach (var item in items)
            {
                if(item.restaurant_id == _usrObj.RestaurantId)
                {
                    list.Add(new SelectListItem() { Value = item.restaurant_id.ToString(), Text = item.restaurant_name, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem() { Value = item.restaurant_id.ToString(), Text = item.restaurant_name });
                }
                
            }

            result.Success = true;
            result.Data = list;

            return result;
        }

        [HttpGet]
        public async Task<JsonInfoResult> RestaurantChange(int id)
        {
            var result = new JsonInfoResult();

            _usrObj.RestaurantId = id;

            result.Success = true;

            return result;
        }

        public JsonInfoResult GetPermissions()
        {
            var info = new JsonInfoResult();

            try
            {
                info.Data = AuthManager.PermissionManager.ToList();
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
