using AdminPortalV8.Data;
using AdminPortalV8.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPortalV8.Models.Analytic;
using Syncfusion.UI.Xaml.Charts;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;


namespace AdminPortalV8.Services
{
    public interface IAnalytic
    {
        Task<List<SelectListItem>> GetRestaurantsAsSelectList();
        Task<List<SelectListItem>> GetRobotsAsSelectList();
        Task<int> GetTotalCompletedOrders(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<int> GetTotalServedRequests(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<double> GetAverageOrderRequestTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId);

        Task<List<BarChartViewModel>> GetCustomOrdersBarChart(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetTodayOrdersBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetWeeklyOrdersBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetMonthlyOrdersBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetYearlyOrdersBarChart(int restaurantId, int robotId);


        Task<List<BarChartViewModel>> GetCustomRequestsBarChart(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetTodayRequestsBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetWeeklyRequestsBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetMonthlyRequestsBarChart(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetYearlyRequestsBarChart(int restaurantId, int robotId);


        Task<List<BarChartViewModel>> GetCustomAvgOrderTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetTodayAvgOrderTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetWeeklyAvgOrderTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetMonthlyAvgOrderTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetYearlyAvgOrderTime(int restaurantId, int robotId);


        Task<List<BarChartViewModel>> GetCustomAvgRequestTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetTodayAvgRequestTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetWeeklyAvgRequestTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetMonthlyAvgRequestTime(int restaurantId, int robotId);
        Task<List<BarChartViewModel>> GetYearlyAvgRequestTime(int restaurantId, int robotId);



    }

    public class AnalyticServices : IAnalytic
    {
        private readonly ApplicationDbContext _context;

        public AnalyticServices(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetRestaurantsAsSelectList()
        {
            try
            {
                var restaurants = await _context.Restaurants.ToListAsync();
                return restaurants.Select(r => new SelectListItem { Value = r.restaurant_id.ToString(), Text = r.restaurant_name }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching restaurants as SelectList.", ex);
            }
        }

        public async Task<List<SelectListItem>> GetRobotsAsSelectList()
        {
            try
            {
                var robots = await _context.Robots.ToListAsync();
                return robots.Select(r => new SelectListItem { Value = r.robot_id.ToString(), Text = r.robot_name }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while fetching robots as SelectList.", ex);
            }
        }


        public async Task<int> GetTotalCompletedOrders(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var order = await (from o in _context.Orders
                                   join r in _context.Robots on o.robot_id equals r.robot_id
                                   join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                   where o.delivery_starttime.Date >= startDate.Date && o.delivery_starttime.Date <= endDate.Date
                                   && r.active == 1 && rr.active == 1
                                   select new {o,r,rr}).ToListAsync();

                var totalCompletedOrders = 0;

                if(restaurantId == 0)
                {
                    totalCompletedOrders = order.Count;
                }
                else if(robotId == 0)
                {
                    totalCompletedOrders = order.Where(p => p.rr.restaurant_id == restaurantId).Count();
                }
                else
                {
                    totalCompletedOrders = order.Where(p => p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId).Count();
                }

                return totalCompletedOrders;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GetTotalServedRequests(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                int totalServedRequests = 0;

                var request = await (from o in _context.AdditionalRequests
                                     join r in _context.Robots on o.robot_id equals r.robot_id
                                     join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                     where r.active == 1 && rr.active == 1
                                     && o.delivery_starttime.Date >= startDate.Date && o.delivery_endtime.Date <= endDate.Date
                                     select new { o, r, rr }).ToListAsync();

                if(restaurantId == 0)
                {
                    totalServedRequests = request.Count();
                }
                else if(robotId == 0)
                {
                    totalServedRequests = request.Where(p=>p.rr.restaurant_id == restaurantId).Count();
                }
                else
                {
                    totalServedRequests = request.Where(p => p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId).Count();
                }

                return totalServedRequests;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<double> GetAverageOrderRequestTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var totalOrderDuration = 0.00;

                var totalRequestDuration = 0.00;

                int totalOrders = 0;

                int totalRequests = 0;

                var order = await (from o in _context.Orders
                                   join r in _context.Robots on o.robot_id equals r.robot_id
                                   join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                   where o.delivery_starttime.Date >= startDate.Date && o.delivery_starttime.Date <= endDate.Date
                                   && r.active == 1 && rr.active == 1
                                   select new
                                   {
                                       Order = o,
                                       Robot = r,
                                       Restaurant = rr,
                                       Duration = (o.delivery_endtime - o.delivery_starttime).TotalSeconds
                                   }).ToListAsync();

                var request = await (from o in _context.AdditionalRequests
                                     join r in _context.Robots on o.robot_id equals r.robot_id
                                     join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                     where r.active == 1 && rr.active == 1
                                     && o.delivery_starttime.Date >= startDate.Date && o.delivery_endtime.Date <= endDate.Date
                                     select new
                                     {
                                         Order = o,
                                         Robot = r,
                                         Restaurant = rr,
                                         Duration = (o.delivery_endtime - o.delivery_starttime).TotalSeconds
                                     }).ToListAsync();

                if (restaurantId == 0)
                {
                    totalOrderDuration = order.Sum(p => p.Duration);
                    totalRequestDuration = request.Sum(p => p.Duration);
                    totalOrders = order.Count();
                    totalRequests = request.Count();
                }
                else if(robotId == 0)
                {
                    totalOrderDuration = order.Where(p=>p.Restaurant.restaurant_id == restaurantId).Sum(p => p.Duration);
                    totalRequestDuration = request.Where(p=>p.Restaurant.restaurant_id == restaurantId).Sum(p => p.Duration);
                    totalOrders = order.Where(p=>p.Restaurant.restaurant_id == restaurantId).Count();
                    totalRequests = request.Where(p=>p.Restaurant.restaurant_id == restaurantId).Count();
                }
                else
                {
                    totalOrderDuration = order.Where(p => p.Restaurant.restaurant_id == restaurantId && p.Robot.robot_id == robotId).Sum(p => p.Duration);
                    totalRequestDuration = request.Where(p => p.Restaurant.restaurant_id == restaurantId && p.Robot.robot_id == robotId).Sum(p => p.Duration);
                    totalOrders = order.Where(p => p.Restaurant.restaurant_id == restaurantId && p.Robot.robot_id == robotId).Count();
                    totalRequests = request.Where(p => p.Restaurant.restaurant_id == restaurantId && p.Robot.robot_id == robotId).Count();

                }



                double overallAverageTime = (totalOrderDuration + totalRequestDuration) / (totalOrders + totalRequests);
                if(overallAverageTime > 0)
                {
                    TimeSpan totalDuration = TimeSpan.FromSeconds(overallAverageTime);
                    overallAverageTime = totalDuration.TotalMinutes;
                }
                else
                {
                    overallAverageTime = 0;
                }

                return overallAverageTime;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while calculating average order request time.", ex);
            }
        }



        public async Task<List<BarChartViewModel>> GetCustomOrdersBarChart(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var order = await (from o in _context.Orders
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == date.Date
                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = order.Count().ToString()
                        };
                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = order.Where(p => p.rr.restaurant_id == restaurantId).Count().ToString()
                        };
                        list.Add(data);
                    }
                    else
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = order.Where(p => p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId).Count().ToString()
                        };
                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetTodayOrdersBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var today = DateTime.Now;

                for (int i = 0; i < 24; i++)
                {
                    DateTime hour = today.Date.AddHours(i); 
                    var order = await (from o in _context.Orders
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == hour.Date
                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in order
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetWeeklyOrdersBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddDays(-6);

                for (var i = 0; i < 7; i++)
                {
                    DateTime date = startDay.AddDays(i);
                    var order = await (from o in _context.Orders
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == date.Date

                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in order
                                    group p by p.o.delivery_starttime.ToString("dd/MM/yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetMonthlyOrdersBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    DateTime date = startDay.AddMonths(i);

                    var order = await (from o in _context.Orders
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date.Month == date.Month &&
                                       o.delivery_starttime.Date.Year == date.Year

                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in order
                                    group p by p.o.delivery_starttime.ToString("MMM yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetYearlyOrdersBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var minDate = await _context.Orders.MinAsync(o => o.delivery_starttime);
                var startDate = new DateTime(minDate.Year, 1, 1);
                var endDate = DateTime.Today;

                for (DateTime date = startDate; date <= endDate; date = date.AddYears(1))
                {
                    var order = await (from o in _context.Orders
                                         join r in _context.Robots on o.robot_id equals r.robot_id
                                         join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                         where /*o.delivery_starttime.Date.Month == date.Month &&*/
                                               o.delivery_starttime.Date.Year == date.Year
                                         select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in order
                                    group p by p.o.delivery_starttime.ToString("yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in order
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<BarChartViewModel>> GetCustomRequestsBarChart(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var request = await (from o in _context.AdditionalRequests
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == date.Date
                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = request.Count().ToString()
                        };
                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = request.Where(p => p.rr.restaurant_id == restaurantId).Count().ToString()
                        };
                        list.Add(data);
                    }
                    else
                    {
                        var data = new BarChartViewModel
                        {
                            BarLabel = date.ToString("dd/MM/yyyy"),
                            BarData = request.Where(p => p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId).Count().ToString()
                        };
                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetTodayRequestsBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var today = DateTime.Now;

                for (int i = 0; i < 24; i++)
                {
                    DateTime hour = today.Date.AddHours(i);
                    var request = await (from o in _context.AdditionalRequests
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == hour.Date
                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in request
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.ToString("hh:00 tt") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = hour.ToString("hh:00 tt"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetWeeklyRequestsBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddDays(-6);

                for (var i = 0; i < 7; i++)
                {
                    DateTime date = startDay.AddDays(i);
                    var request = await (from o in _context.AdditionalRequests
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where o.delivery_starttime.Date == date.Date

                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in request
                                    group p by p.o.delivery_starttime.ToString("dd/MM/yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("dd/MM/yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetMonthlyRequestsBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    DateTime date = startDay.AddMonths(i);

                    var request = await (from o in _context.AdditionalRequests
                                         join r in _context.Robots on o.robot_id equals r.robot_id
                                         join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                         where o.delivery_starttime.Date.Month == date.Month &&
                                         o.delivery_starttime.Date.Year == date.Year

                                         select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in request
                                    group p by p.o.delivery_starttime.ToString("MMM yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("MMM yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetYearlyRequestsBarChart(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var minDate = await _context.AdditionalRequests.MinAsync(o => o.delivery_starttime);
                var startDate = new DateTime(minDate.Year, 1, 1);
                var endDate = DateTime.Today;

                for (DateTime date = startDate; date <= endDate; date = date.AddYears(1))
                {
                    var request = await (from o in _context.AdditionalRequests
                                       join r in _context.Robots on o.robot_id equals r.robot_id
                                       join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                       where /*o.delivery_starttime.Date.Month == date.Month &&*/
                                             o.delivery_starttime.Date.Year == date.Year
                                       select new { o, r, rr }).ToListAsync();

                    if (restaurantId == 0)
                    {
                        var data = (from p in request
                                    group p by p.o.delivery_starttime.ToString("yyyy") into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else if (robotId == 0)
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                    else
                    {
                        var data = (from p in request
                                    where p.rr.restaurant_id == restaurantId && p.r.robot_id == robotId
                                    group p by p.o.delivery_starttime.Date into g
                                    select new BarChartViewModel()
                                    {
                                        BarLabel = g.Key.ToString(),
                                        BarData = g.Count().ToString()
                                    }).FirstOrDefault();

                        if (data == null)
                        {
                            data = new BarChartViewModel()
                            {
                                BarLabel = date.ToString("yyyy"),
                                BarData = "0"
                            };
                        }

                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<BarChartViewModel>> GetCustomAvgOrderTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    double totalDuration = 0.00;
                    int orderCount = 0;

                    var ordersQuery = from o in _context.Orders
                                      join r in _context.Robots on o.robot_id equals r.robot_id
                                      join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                      where o.delivery_starttime.Date == date.Date
                                      select new
                                      {
                                          o.delivery_endtime,
                                          o.delivery_starttime,
                                          rr.restaurant_id,
                                          r.robot_id
                                      };

                    if (restaurantId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.robot_id == robotId);
                    }

                    var orders = await ordersQuery.ToListAsync();

                    orderCount = orders.Count;
                    totalDuration = orders.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgOrderTimeInSeconds = (orderCount == 0) ? 0 : totalDuration / orderCount;
                    double avgOrderTimeInMinutes = (avgOrderTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgOrderTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("dd/MM/yyyy"),
                        BarData = avgOrderTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetTodayAvgOrderTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var today = DateTime.Now.Date;
                var tomorrow = today.AddDays(1);

                var ordersQuery = from o in _context.Orders
                                  join r in _context.Robots on o.robot_id equals r.robot_id
                                  join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                  where o.delivery_starttime >= today && o.delivery_starttime < tomorrow
                                  select new
                                  {
                                      o.delivery_endtime,
                                      o.delivery_starttime,
                                      rr.restaurant_id,
                                      r.robot_id
                                  };

                if (restaurantId != 0)
                {
                    ordersQuery = ordersQuery.Where(o => o.restaurant_id == restaurantId);
                }

                if (robotId != 0)
                {
                    ordersQuery = ordersQuery.Where(o => o.robot_id == robotId);
                }

                var orders = await ordersQuery.ToListAsync();

                for (int hour = 0; hour < 24; hour++)
                {
                    double totalDuration = 0.00;
                    int orderCount = 0;

                    var ordersInHour = orders.Where(o => o.delivery_starttime.Hour == hour).ToList();
                    orderCount = ordersInHour.Count;
                    totalDuration = ordersInHour.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgOrderTimeInSeconds = (orderCount == 0) ? 0 : totalDuration / orderCount;
                    double avgOrderTimeInMinutes = (avgOrderTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgOrderTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = today.AddHours(hour).ToString("hh:00 tt"),
                        BarData = avgOrderTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetWeeklyAvgOrderTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddDays(-6);

                for (var i = 0; i < 7; i++)
                {
                    double totalDuration = 0.00;
                    int orderCount = 0;

                    DateTime date = startDay.AddDays(i);

                    var ordersQuery = from o in _context.Orders
                                      join r in _context.Robots on o.robot_id equals r.robot_id
                                      join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                      where o.delivery_starttime.Date == date.Date
                                      select new
                                      {
                                          o.delivery_endtime,
                                          o.delivery_starttime,
                                          rr.restaurant_id,
                                          r.robot_id
                                      };

                    if (restaurantId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.robot_id == robotId);
                    }

                    var orders = await ordersQuery.ToListAsync();

                    orderCount = orders.Count;
                    totalDuration = orders.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgOrderTimeInSeconds = (orderCount == 0) ? 0 : totalDuration / orderCount;
                    double avgOrderTimeInMinutes = (avgOrderTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgOrderTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("dd/MM/yyyy"),
                        BarData = avgOrderTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetMonthlyAvgOrderTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    double totalDuration = 0.00;
                    int orderCount = 0;

                    DateTime date = startDay.AddMonths(i);

                    var ordersQuery = from o in _context.Orders
                                      join r in _context.Robots on o.robot_id equals r.robot_id
                                      join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                      where o.delivery_starttime.Month == date.Month && o.delivery_starttime.Year == date.Year
                                      select new
                                      {
                                          o.delivery_endtime,
                                          o.delivery_starttime,
                                          rr.restaurant_id,
                                          r.robot_id
                                      };

                    if (restaurantId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.robot_id == robotId);
                    }

                    var orders = await ordersQuery.ToListAsync();

                    orderCount = orders.Count;
                    totalDuration = orders.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgOrderTimeInSeconds = (orderCount == 0) ? 0 : totalDuration / orderCount;
                    double avgOrderTimeInMinutes = (avgOrderTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgOrderTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("MMM yyyy"),
                        BarData = avgOrderTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetYearlyAvgOrderTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var minDate = await _context.Orders.MinAsync(o => (DateTime?)o.delivery_starttime);

                if (minDate == null)
                {
                    throw new Exception("No orders found in the database.");
                }

                var startDate = new DateTime(minDate.Value.Year, 1, 1);
                var endDate = DateTime.Today;

                for (DateTime year = startDate; year.Year <= endDate.Year; year = year.AddYears(1))
                {
                    double totalDuration = 0.00;
                    int orderCount = 0;

                    var ordersQuery = from o in _context.Orders
                                      join r in _context.Robots on o.robot_id equals r.robot_id
                                      join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                      where o.delivery_starttime.Year == year.Year
                                      select new
                                      {
                                          o.delivery_endtime,
                                          o.delivery_starttime,
                                          rr.restaurant_id,
                                          r.robot_id
                                      };

                    if (restaurantId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        ordersQuery = ordersQuery.Where(o => o.robot_id == robotId);
                    }

                    var ordersOfYear = await ordersQuery.ToListAsync();

                    orderCount = ordersOfYear.Count;
                    totalDuration = ordersOfYear.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgOrderTimeInSeconds = (orderCount == 0) ? 0 : totalDuration / orderCount;
                    double avgOrderTimeInMinutes = (avgOrderTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgOrderTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = year.Year.ToString(),
                        BarData = avgOrderTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<BarChartViewModel>> GetCustomAvgRequestTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    double totalDuration = 0.00;
                    int requestCount = 0;

                    var requestsQuery = from o in _context.AdditionalRequests
                                        join r in _context.Robots on o.robot_id equals r.robot_id
                                        join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                        where o.delivery_starttime.Date == date.Date
                                        select new
                                        {
                                            o.delivery_endtime,
                                            o.delivery_starttime,
                                            rr.restaurant_id,
                                            r.robot_id
                                        };

                    if (restaurantId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.robot_id == robotId);
                    }

                    var requests = await requestsQuery.ToListAsync();

                    requestCount = requests.Count;
                    totalDuration = requests.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgRequestTimeInSeconds = (requestCount == 0) ? 0 : totalDuration / requestCount;
                    double avgRequestTimeInMinutes = (avgRequestTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgRequestTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("dd/MM/yyyy"),
                        BarData = avgRequestTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetTodayAvgRequestTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var today = DateTime.Now.Date;
                var tomorrow = today.AddDays(1);

                var requestsQuery = from o in _context.AdditionalRequests
                                    join r in _context.Robots on o.robot_id equals r.robot_id
                                    join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                    where o.delivery_starttime >= today && o.delivery_starttime < tomorrow
                                    select new
                                    {
                                        o.delivery_endtime,
                                        o.delivery_starttime,
                                        rr.restaurant_id,
                                        r.robot_id
                                    };

                if (restaurantId != 0)
                {
                    requestsQuery = requestsQuery.Where(o => o.restaurant_id == restaurantId);
                }

                if (robotId != 0)
                {
                    requestsQuery = requestsQuery.Where(o => o.robot_id == robotId);
                }

                var requests = await requestsQuery.ToListAsync();

                for (int hour = 0; hour < 24; hour++)
                {
                    double totalDuration = 0.00;
                    int requestCount = 0;

                    var requestsInHour = requests.Where(o => o.delivery_starttime.Hour == hour).ToList();
                    requestCount = requestsInHour.Count;
                    totalDuration = requestsInHour.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgRequestTimeInSeconds = (requestCount == 0) ? 0 : totalDuration / requestCount;
                    double avgRequestTimeInMinutes = (avgRequestTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgRequestTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = today.AddHours(hour).ToString("hh:00 tt"),
                        BarData = avgRequestTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetWeeklyAvgRequestTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddDays(-6);

                for (var i = 0; i < 7; i++)
                {
                    double totalDuration = 0.00;
                    int requestCount = 0;

                    DateTime date = startDay.AddDays(i);

                    var requestsQuery = from o in _context.AdditionalRequests
                                        join r in _context.Robots on o.robot_id equals r.robot_id
                                        join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                        where o.delivery_starttime.Date == date.Date
                                        select new
                                        {
                                            o.delivery_endtime,
                                            o.delivery_starttime,
                                            rr.restaurant_id,
                                            r.robot_id
                                        };

                    if (restaurantId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.robot_id == robotId);
                    }

                    var requests = await requestsQuery.ToListAsync();

                    requestCount = requests.Count;
                    totalDuration = requests.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgRequestTimeInSeconds = (requestCount == 0) ? 0 : totalDuration / requestCount;
                    double avgRequestTimeInMinutes = (avgRequestTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgRequestTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("dd/MM/yyyy"),
                        BarData = avgRequestTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetMonthlyAvgRequestTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var startDay = DateTime.Now.AddMonths(-11);

                for (int i = 0; i < 12; i++)
                {
                    double totalDuration = 0.00;
                    int requestCount = 0;

                    DateTime date = startDay.AddMonths(i);

                    var requestsQuery = from o in _context.AdditionalRequests
                                        join r in _context.Robots on o.robot_id equals r.robot_id
                                        join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                        where o.delivery_starttime.Month == date.Month && o.delivery_starttime.Year == date.Year
                                        select new
                                        {
                                            o.delivery_endtime,
                                            o.delivery_starttime,
                                            rr.restaurant_id,
                                            r.robot_id
                                        };

                    if (restaurantId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.robot_id == robotId);
                    }

                    var requests = await requestsQuery.ToListAsync();

                    requestCount = requests.Count;
                    totalDuration = requests.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgRequestTimeInSeconds = (requestCount == 0) ? 0 : totalDuration / requestCount;
                    double avgRequestTimeInMinutes = (avgRequestTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgRequestTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = date.ToString("MMM yyyy"),
                        BarData = avgRequestTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BarChartViewModel>> GetYearlyAvgRequestTime(int restaurantId, int robotId)
        {
            try
            {
                var list = new List<BarChartViewModel>();
                var minDate = await _context.AdditionalRequests.MinAsync(o => (DateTime?)o.delivery_starttime);
                var startDate = new DateTime(minDate.Value.Year, 1, 1);
                var endDate = DateTime.Today;

                for (DateTime year = startDate; year.Year <= endDate.Year; year = year.AddYears(1))
                {
                    double totalDuration = 0.00;
                    int requestCount = 0;

                    var requestsQuery = from o in _context.AdditionalRequests
                                        join r in _context.Robots on o.robot_id equals r.robot_id
                                        join rr in _context.Restaurants on r.restaurant_id equals rr.restaurant_id
                                        where o.delivery_starttime.Year == year.Year
                                        select new
                                        {
                                            o.delivery_endtime,
                                            o.delivery_starttime,
                                            rr.restaurant_id,
                                            r.robot_id
                                        };

                    if (restaurantId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.restaurant_id == restaurantId);
                    }

                    if (robotId != 0)
                    {
                        requestsQuery = requestsQuery.Where(o => o.robot_id == robotId);
                    }

                    var requestsOfYear = await requestsQuery.ToListAsync();

                    requestCount = requestsOfYear.Count;
                    totalDuration = requestsOfYear.Sum(o => (o.delivery_endtime - o.delivery_starttime).TotalSeconds);

                    double avgRequestTimeInSeconds = (requestCount == 0) ? 0 : totalDuration / requestCount;
                    double avgRequestTimeInMinutes = (avgRequestTimeInSeconds > 0) ? TimeSpan.FromSeconds(avgRequestTimeInSeconds).TotalMinutes : 0;

                    list.Add(new BarChartViewModel
                    {
                        BarLabel = year.Year.ToString(),
                        BarData = avgRequestTimeInMinutes.ToString("F2")
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
