using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Models;
using AdminPortalV8.Models.Restaurant;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static AdminPortalV8.Helpers.AppModuleKeys;


namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurant _restaurant;
        private readonly RestaurantObj _obj;
        //private readonly ApplicationDbContext _context;

        public RestaurantsController(IRestaurant restaurant, RestaurantObj objs)/*, ApplicationDbContext context)*/
        {
            _restaurant = restaurant;
            _obj = objs;
            //_context=context;
        }

        [ContentPermission(
            Key = AppModuleKeys.Restaurant.ViewPrimaryKey,
            Title = AppModuleKeys.Restaurant.ViewTitle,
            Desc = AppModuleKeys.Restaurant.ViewDescription,
            StaticAuthorization = AppModuleKeys.Restaurant.ViewStaticAuthorized
            )]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = false;
            var list = await _restaurant.GetListRestaurant();
            var model = new RestaurantView();

            model.RestaurantList = list;

            _obj.Robots = new List<RobotModel>();
            _obj.Anchors = new List<AnchorModel>();

            return View(model);
        }

        
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonInfoResult> AddRobot(RestaurantDetailModel model)
        {
            var info = new JsonInfoResult();

            var data = model.RobotDetail;

            if (data.restaurant_id > 0)
            {
                await _restaurant.AddRobotRestaurant(data);
            }
            else
            {
                _obj.Robots.Add(new RobotModel()
                {
                    Active = data.Active,
                    Name = data.Name,
                    restaurant_id = 0,
                    SerialNum = data.SerialNum,
                    Id = _obj.Robots.Count == 0 ? 1 : _obj.Robots.LastOrDefault().Id + 1
                });
            }

            info.Success = true;
            await Task.CompletedTask;

            return info;
        }

        [HttpPost]
        public async Task<JsonInfoResult> AddAnchor(RestaurantDetailModel model)
        {
            var info = new JsonInfoResult();

            var data = model.AnchorDetail;

            if (data.restaurant_id > 0)
            {
                await _restaurant.AddAnchorRestaurant(data);
            }
            else
            {
                _obj.Anchors.Add(new AnchorModel()
                {
                    restaurant_id = 0,
                    Id = _obj.Anchors.Count == 0 ? 1 : _obj.Anchors.LastOrDefault().Id + 1,
                    Anchor_Address = data.Anchor_Address,
                    isMainAnchor = data.isMainAnchor,
                    X_Axis = data.X_Axis,
                    Y_Axis = data.Y_Axis
                });
            }

            info.Success = true;
            await Task.CompletedTask;

            return info;
        }

        [HttpGet]
        public async Task<JsonResult> GetRobots(int restId, DataTablePagingFilter filter)
        {
            var result = new JsonInfoResult();
            var dto = new Paging<RobotModel>();
            try
            {
                if(restId == 0)
                {
                    var data = _obj.Robots;
                    dto.List = data;
                }
                else
                {
                    var data = await _restaurant.GetRobotListByRestId(restId);
                    dto.List = data;
                }

                return DataTableSerializer.SerializeDataTable<RobotModel>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetAnchors(int restId, DataTablePagingFilter filter)
        {
            var result = new JsonInfoResult();
            var dto = new Paging<AnchorModel>();
            try
            {
                if (restId == 0)
                {
                    var data = _obj.Anchors;
                    dto.List = data;
                }
                else
                {
                    var data = await _restaurant.GetAnchorListByRestId(restId);
                    dto.List = data;
                }

                return DataTableSerializer.SerializeDataTable<AnchorModel>(filter.sEcho, dto);
            }
            catch (Exception ex)
            {
                result.Data = ex.ToString();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveRobot(int Id, int restId)
        {
            var info = new JsonInfoResult();

            try
            {
                if(restId == 0)
                {
                    var rbt = _obj.Robots.FirstOrDefault(p => p.Id == Id);
                    _obj.Robots.Remove(rbt);
                }
                else
                {
                    await _restaurant.RemoveRobot(Id);
                }

                info.Success = true;
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                info.Data = ex.ToString();
            }

            return Json(info);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveAnchor(int Id, int restId)
        {
            var info = new JsonInfoResult();

            try
            {
                if (restId == 0)
                {
                    var anc = _obj.Anchors.FirstOrDefault(p => p.Id == Id);
                    _obj.Anchors.Remove(anc);
                }
                else
                {
                    await _restaurant.RemoveAnchor(Id);
                }

                info.Success = true;
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                info.Data = ex.ToString();
            }

            return Json(info);
        }


        public async Task<IActionResult> Edit(int uid)
        {
            var model = await _restaurant.GetRestaurantDetail(uid);
            model.Robots = await _restaurant.GetRobotListByRestId(uid);
            model.Anchors = await _restaurant.GetAnchorListByRestId(uid);
            return View(model);
        }

        public async Task<IActionResult> Detail(int uid)
        {
            var model = await _restaurant.GetRestaurantDetail(uid);
            model.Robots = await _restaurant.GetRobotListByRestId(uid);
            model.Anchors = await _restaurant.GetAnchorListByRestId(uid);
            return View(model);
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.Restaurant.CreatePrimaryKey,
            Title = AppModuleKeys.Restaurant.CreateTitle,
            Desc = AppModuleKeys.Restaurant.CreateDescription,
            StaticAuthorization = AppModuleKeys.Restaurant.CreateStaticAuthorized,
            AssociatedKey = AppModuleKeys.Restaurant.ViewPrimaryKey
            )]
        public async Task<JsonInfoResult> Create(RestaurantDetailModel restaurant)
        {
            var info = new JsonInfoResult();
            if (ModelState.IsValid)
            {
                
                    var result = await _restaurant.AddOrUpdateRestaurant(restaurant);
                    if (result)
                    {

                    TempData["msg"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Added succesfully.\"});</script>";
                    
                    }
                    else
                    {
                    TempData["msg"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Added failed.\"});</script>";

                  
                    }
                
            }
            info.Success = true;
            return info;
        }

        [HttpGet]
        [ContentPermission(
            Key = AppModuleKeys.Restaurant.RemovePrimaryKey,
            Title = AppModuleKeys.Restaurant.RemoveTitle,
            Desc = AppModuleKeys.Restaurant.RemoveDescription,
            StaticAuthorization = AppModuleKeys.Restaurant.RemoveStaticAuthorized,
            AssociatedKey = AppModuleKeys.Restaurant.ViewPrimaryKey
            )]
        public async Task<ActionResult> Delete(int Id)
        {
            await _restaurant.DeleteRestaurant(Id);

            TempData["msg"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Deleted succesfully.\"});</script>";
            //TempData["msg"] = "<script>alert('Deleted succesfully');</script>";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ContentPermission(
            Key = AppModuleKeys.Restaurant.EditPrimaryKey,
            Title = AppModuleKeys.Restaurant.EditTitle,
            Desc = AppModuleKeys.Restaurant.EditDescription,
            StaticAuthorization = AppModuleKeys.Restaurant.EditStaticAuthorized,
            AssociatedKey = AppModuleKeys.Restaurant.ViewPrimaryKey
            )]
        public async Task<ActionResult> Edit(RestaurantDetailModel restaurant)
        {
            if (ModelState.IsValid)
            {
                var result = await _restaurant.AddOrUpdateRestaurant(restaurant);
                if (result)
                {
                    TempData["msg"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Updated succesfully.\"});</script>";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["msg"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Updated failed.\"});</script>";

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(restaurant);
        }

    }
}

