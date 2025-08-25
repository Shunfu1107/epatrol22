using AdminPortalV8.Data;
using AdminPortalV8.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Migrations;
using AdminPortalV8.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.WebPages;

namespace AdminPortalV8.Services
{
    public interface IDashboardService
    {
        Task<int> GetTotalRestaurantsByUserId(int userId);
        Task<int> GetTotalRobotsByUserId(int userId);
        Task<List<int>> GetWeeklyOrdersByUserRestaurant(int userId, int restaurantId);
        Task<List<int>> GetWeeklyRequestByUserRestaurant(int userId, int restaurantId);
        Task<List<int>> GetYearlyOrdersByUserRestaurant(int userId, int restaurantId);
        Task<List<int>> GetYearlyRequestByUserRestaurant(int userId, int restaurantId);

        Task<Paging<UserRestaurantModel>> GetUserRestaurant(int UserID);

    }

    public class DashboardService : IDashboardService
    {
        private ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalRestaurantsByUserId(int userId)
        {
            try
            {
                var item = 0;
                if(userId == 0)
                {
                    item = await _context.Restaurants.CountAsync();
                }
                else
                {
                    item = await (from ur in _context.userRestaurants
                                  join r in _context.Restaurants on ur.restaurantId equals r.restaurant_id
                                  where ur.userId == userId
                                  select r).CountAsync();
                }

                return item;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalRobotsByUserId(int userId)
        {
            try
            {
                var item = 0;
                if (userId == 0)
                {
                    item = await _context.Robots.CountAsync();
                }
                else
                {
                    item = await(from ur in _context.userRestaurants
                                 join r in _context.Restaurants on ur.restaurantId equals r.restaurant_id
                                 join ro in _context.Robots on r.restaurant_id equals ro.restaurant_id
                                 where ur.userId == userId
                                 select r).CountAsync();
                }

                return item;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<Paging<UserRestaurantModel>> GetUserRestaurant(int UserID)
        {
            var dto = new Paging<UserRestaurantModel>();
            try
            {
                var result = await(from ur in _context.userRestaurants
                                   join r in _context.Restaurants on ur.restaurantId equals r.restaurant_id
                                   where ur.userId == UserID
                                   select new UserRestaurantModel()
                                   {
                                       pvid = ur.Id,
                                       userID = ur.userId,
                                       restaurant_id = ur.restaurantId,
                                       restaurant_name = r.restaurant_name,
                                       StartActiveDate = ur.Created,
                                       Status = r.active == 1 ? "Active" : "Inactive"
                                   }).ToListAsync();

                dto.List = result;
            }
            catch (Exception ex)
            {

            }
            return dto;
        }

        public async Task<List<int>> GetWeeklyOrdersByUserRestaurant(int userId, int restaurantId)
        {
            try
            {
                var item = new List<int>();
                var startDay = DateTime.Now.AddDays(-6);

                for(var i = 0; i < 7; i++)
                {
                    DateTime date = startDay.AddDays(i);
                    var data = await (from o in _context.Orders
                                      join ro in _context.Robots on o.robot_id equals ro.robot_id
                                      join r in _context.Restaurants on ro.restaurant_id equals r.restaurant_id
                                      where o.delivery_starttime.Date == date.Date
                                       
                                      select new { o, ro, r }).ToListAsync();

                    if(userId > 0)
                    {
                        data = (from d in data
                                join ur in _context.userRestaurants on d.r.restaurant_id equals ur.restaurantId
                                where ur.userId == userId
                                select d).ToList();
                    }

                    if(restaurantId > 0)
                    {
                        data = data.Where(p => p.r.restaurant_id == restaurantId).ToList();
                    }

                    item.Add(data.Count());
                }
               

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int>> GetWeeklyRequestByUserRestaurant(int userId, int restaurantId)
        {
            try
            {
                var item = new List<int>();
                var startDay = DateTime.Now.AddDays(-6);

                for (var i = 0; i < 7; i++)
                {
                    DateTime date = startDay.AddDays(i);
                    var data = await(from o in _context.AdditionalRequests
                                     join ro in _context.Robots on o.robot_id equals ro.robot_id
                                     join r in _context.Restaurants on ro.restaurant_id equals r.restaurant_id
                                     where o.delivery_starttime.Date == date.Date

                                     select new { o, ro, r }).ToListAsync();

                    if (userId > 0)
                    {
                        data = (from d in data
                                join ur in _context.userRestaurants on d.r.restaurant_id equals ur.restaurantId
                                where ur.userId == userId
                                select d).ToList();
                    }

                    if (restaurantId > 0)
                    {
                        data = data.Where(p => p.r.restaurant_id == restaurantId).ToList();
                    }

                    item.Add(data.Count());
                }


                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int>> GetYearlyOrdersByUserRestaurant(int userId, int restaurantId)
        {
            try
            {
                var item = new List<int>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    DateTime date = startDay.AddMonths(i);

                    var data = await (from o in _context.Orders
                                      join ro in _context.Robots on o.robot_id equals ro.robot_id
                                      join r in _context.Restaurants on ro.restaurant_id equals r.restaurant_id
                                      where o.delivery_starttime.Date.Month == date.Month &&
                                      o.delivery_starttime.Date.Year == date.Year

                                      select new { o, ro, r }).ToListAsync();

                    if (userId > 0)
                    {
                        data = (from d in data
                                join ur in _context.userRestaurants on d.r.restaurant_id equals ur.restaurantId
                                where ur.userId == userId
                                select d).ToList();
                    }

                    if (restaurantId > 0)
                    {
                        data = data.Where(p => p.r.restaurant_id == restaurantId).ToList();
                    }

                    item.Add(data.Count);
                }


                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int>> GetYearlyRequestByUserRestaurant(int userId, int restaurantId)
        {
            try
            {
                var item = new List<int>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    DateTime date = startDay.AddMonths(i);

                    var data = await(from o in _context.AdditionalRequests
                                     join ro in _context.Robots on o.robot_id equals ro.robot_id
                                     join r in _context.Restaurants on ro.restaurant_id equals r.restaurant_id
                                     where o.delivery_starttime.Date.Month == date.Month &&
                                     o.delivery_starttime.Date.Year == date.Year

                                     select new { o, ro, r }).ToListAsync();

                    if (userId > 0)
                    {
                        data = (from d in data
                                join ur in _context.userRestaurants on d.r.restaurant_id equals ur.restaurantId
                                where ur.userId == userId
                                select d).ToList();
                    }

                    if (restaurantId > 0)
                    {
                        data = data.Where(p => p.r.restaurant_id == restaurantId).ToList();
                    }

                    item.Add(data.Count);
                }


                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
