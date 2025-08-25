using AdminPortalV8.Data;
using AdminPortalV8.Entities;
using AdminPortalV8.Models;
using AdminPortalV8.Models.Restaurant;
using Microsoft.EntityFrameworkCore;


namespace AdminPortalV8.Services
{
    public interface IRestaurant
    {
        Task<List<RestaurantModel>> GetListRestaurant();
        Task<bool> AddUserRestaurant(AddUserRestaurant model);
        Task<bool> RemoveUserRestaurant(int pvid);
        Task<List<UserRestaurantModel>> GetUserRestaurantList(int userId);
        Task<bool> AddOrUpdateRestaurant(RestaurantDetailModel restaurant);
        Task<bool> DeleteRestaurant(int RestaurantId);
        Task<RestaurantDetailModel> GetRestaurantDetail(int id);
        Task<List<RobotModel>> GetRobotListByRestId(int restId);
        Task<List<AnchorModel>> GetAnchorListByRestId(int restId);
        Task<bool> AddRobotRestaurant(RobotModel model);
        Task<bool> AddAnchorRestaurant(AnchorModel model);
        Task<bool> RemoveRobot(int id);
        Task<bool> RemoveAnchor(int id);
    }

    public class RestaurantServices : IRestaurant
    {
        private readonly ApplicationDbContext _context;

        public RestaurantServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserRestaurant(AddUserRestaurant model)
        {
            try
            {
                var item = _context.userRestaurants.FirstOrDefault(p => p.userId == model.userId && p.restaurantId == model.restaurantId);
                if(item == null)
                {
                    _context.Add(new UserRestaurant()
                    {
                        userId = model.userId,
                        restaurantId = model.restaurantId
                    });

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

        public async Task<List<RestaurantModel>> GetListRestaurant()
        {
            try
            {
                var items = await (from r in _context.Restaurants
                                   select new RestaurantModel()
                                   {
                                       Id = r.restaurant_id,
                                       Name = r.restaurant_name,
                                       Location = r.restaurant_address,
                                       StartDate = r.startDate.ToString("dd MMM yyyy"),
                                       Active = r.active == 1 ? "Yes" : "No",
                                       Anchor = _context.Anchors.Count(p => p.restaurant_id == r.restaurant_id),
                                       Robots = _context.Robots.Count(p => p.restaurant_id  == r.restaurant_id)

                                   }).ToListAsync<RestaurantModel>();

                return items;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserRestaurantModel>> GetUserRestaurantList(int userId)
        {
            try
            {
                var result = await (from ur in _context.userRestaurants
                                    join r in _context.Restaurants on ur.restaurantId equals r.restaurant_id
                                    where ur.userId == userId
                                    select new UserRestaurantModel()
                                    {
                                        pvid = ur.Id,
                                        userID = ur.userId,
                                        restaurant_id = r.restaurant_id,
                                        restaurant_name = r.restaurant_name,
                                        StartActiveDate = ur.Created,
                                        Status = r.active == 1 ? "Active" : "Inactive"
                                    }).ToListAsync();

                if(userId == 0)
                {
                    result = await (from r in _context.Restaurants
                                    where r.active == 1
                                    select new UserRestaurantModel()
                                    {

                                        restaurant_id = r.restaurant_id,
                                        restaurant_name = r.restaurant_name,
                                        Status = r.active == 1 ? "Active" : "Inactive"
                                    }).ToListAsync();
                }

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveUserRestaurant(int pvid)
        {
            try
            {
                var item = _context.userRestaurants.FirstOrDefault(p=>p.Id == pvid);
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
                return false;
            }
        }


        public async Task<bool> AddOrUpdateRestaurant(RestaurantDetailModel restaurant)
        {
            if (restaurant.Id == 0)
            {
                var restModel = new Restaurant()
                {
                    restaurant_name = restaurant.Name,
                    restaurant_address = restaurant.Address,
                    manager = 0,
                    active = restaurant.Active == true ? 1 : 0,
                    startDate = DateTime.ParseExact(restaurant.StartDate, "dd/MM/yyyy", null)
                };
                _context.Restaurants.Add(restModel);
                _context.SaveChanges();

                var id = restModel.restaurant_id;

                if(restaurant.Robots != null)
                {
                    foreach (var item in restaurant.Robots)
                    {
                        _context.Robots.Add(new Robot()
                        {
                            created_datetime = DateTime.Now,
                            restaurant_id = id,
                            robot_name = item.Name,
                            robot_serialnum = item.SerialNum,
                            active = item.Active == true ? 1 : 0
                        });
                    }

                    _context.SaveChanges();
                }
                
                
                if(restaurant.Anchors != null)
                {
                    foreach (var item in restaurant.Anchors)
                    {
                        _context.Anchors.Add(new Anchor()
                        {
                            anchor_address = item.Anchor_Address,
                            ismain_anchor = item.isMainAnchor == true ? 1 : 0,
                            X_Axis = item.X_Axis,
                            Y_Axis = item.Y_Axis,
                            restaurant_id = id
                        });
                    }

                    _context.SaveChanges();
                }
               

                await Task.CompletedTask;
            }
            else
            {
                Restaurant _data = _context.Restaurants.FirstOrDefault(p => p.restaurant_id == restaurant.Id);
                if (_data != null)
                {
                    _data.restaurant_name = restaurant.Name;
                    _data.restaurant_address = restaurant.Address;
                    _data.startDate = DateTime.ParseExact(restaurant.StartDate, "dd/MM/yyyy", null);
                    _data.active = restaurant.Active == true ? 1 : 0;

                }
                _context.SaveChanges();
                await Task.CompletedTask;
            }

            return true;
        }

        public async Task<bool> DeleteRestaurant(int RestaurantId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.restaurant_id == RestaurantId);
            
            if(restaurant != null)
            {
                var robot = _context.Robots.Where(p => p.restaurant_id == restaurant.restaurant_id).ToList();
                var anchor = _context.Anchors.Where(p => p.restaurant_id == restaurant.restaurant_id).ToList();
                var ableDelete = true;

                foreach(var item in robot)
                {
                    var ord = _context.Orders.Where(p=>p.robot_id == item.robot_id).ToList();
                    if(ord.Count > 0)
                    {
                        ableDelete = false;
                    }
                }

                if(ableDelete)
                {
                    //Delete robot
                    foreach(var item in robot)
                    {
                        _context.Remove(item);
                    }

                    foreach(var item in anchor)
                    {
                        _context.Remove(item);
                    }

                    _context.Remove(restaurant);
                    
                }
                else
                {
                    restaurant.active = 0;
                    _context.Update(restaurant);
                }

                _context.SaveChanges();
            }

            return true;
        }

        public async Task<RestaurantDetailModel> GetRestaurantDetail(int id)
        {
            try
            {
                var item = await (from r in _context.Restaurants
                                  where r.restaurant_id == id
                                  select new RestaurantDetailModel()
                                  {
                                      Id = r.restaurant_id,
                                      Active = r.active == 1 ? true : false,
                                      Name = r.restaurant_name,
                                      Address = r.restaurant_address,
                                      Manager = r.manager,
                                      StartDate = r.startDate.ToString("dd/MM/yyyy")
                                  }).FirstOrDefaultAsync();

                return item;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<RobotModel>> GetRobotListByRestId(int restId)
        {
            try
            {
                var item = new List<RobotModel>();

                if (restId > 0)
                {
                    item = await (from r in _context.Robots
                                      where r.restaurant_id == restId
                                      select new RobotModel()
                                      {
                                          Id = r.robot_id,
                                          Active = r.active == 1 ? true : false,
                                          Name = r.robot_name,
                                          restaurant_id = r.restaurant_id,
                                          SerialNum = r.robot_serialnum
                                      }).ToListAsync();
                }
                    

               

                return item;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AnchorModel>> GetAnchorListByRestId(int restId)
        {
            try
            {
                var item = await(from r in _context.Anchors
                                 where r.restaurant_id == restId
                                 select new AnchorModel()
                                 {
                                     
                                     restaurant_id = r.restaurant_id,
                                    Id = r.anchor_id,
                                    Anchor_Address = r.anchor_address,
                                    isMainAnchor = r.ismain_anchor == 1 ? true : false,
                                    X_Axis = Convert.ToSingle(r.X_Axis),
                                    Y_Axis = Convert.ToSingle(r.Y_Axis)
                                 }).ToListAsync();

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddRobotRestaurant(RobotModel model)
        {
            try
            {
                _context.Robots.Add(new Robot()
                {
                    active = model.Active == true ? 1 : 0,
                    created_datetime = DateTime.Now,
                    restaurant_id = model.restaurant_id,
                    robot_name = model.Name,
                    robot_serialnum = model.SerialNum
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

        public async Task<bool> AddAnchorRestaurant(AnchorModel model)
        {
            try
            {
                _context.Anchors.Add(new Anchor()
                {
                    restaurant_id = model.restaurant_id,
                    anchor_address = model.Anchor_Address,
                    ismain_anchor = model.isMainAnchor == true ? 1 : 0,
                    X_Axis = Convert.ToSingle(model.X_Axis),
                    Y_Axis = Convert.ToSingle(model.Y_Axis)
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

        public async Task<bool> RemoveRobot(int id)
        {
            try
            {
                var item = await _context.Robots.FirstOrDefaultAsync(p => p.robot_id == id);
                if(item != null)
                {
                    var orders = _context.Orders.Where(p=>p.robot_id ==  item.robot_id).ToList();
                    if(orders.Count > 0)
                    {
                        item.active = 0;
                        _context.Update(item);
                    }
                    else
                    {
                        _context.Remove(item);
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

        public async Task<bool> RemoveAnchor(int id)
        {
            try
            {
                var item = await _context.Anchors.FirstOrDefaultAsync(p => p.anchor_id == id);
                if (item != null)
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
    }
}
    
