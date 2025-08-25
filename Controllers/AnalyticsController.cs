using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Models;
using AdminPortalV8.Models.Restaurant;
using AdminPortalV8.Services;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Models.Analytic;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Helpers;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class AnalyticsController : Controller
    {
        private readonly IAnalytic _analytic;
        private readonly IGeneral _general;
        private readonly IRestaurant _restaurant;
        private readonly AnalyticObj _obj;
        public AnalyticsController(IGeneral general, IRestaurant restaurant, AnalyticObj objs, IAnalytic analytic)
        {
            _general = general;
            _restaurant = restaurant;
            _obj = objs;
            _analytic = analytic;
        }

        [HttpGet]
        [ContentPermission(
            Key = AppModuleKeys.Analytics.ViewPrimaryKey,
            Title = AppModuleKeys.Analytics.ViewTitle,
            Desc = AppModuleKeys.Analytics.ViewDescription,
            StaticAuthorization = AppModuleKeys.Analytics.ViewStaticAuthorized
            )]
        public async Task<IActionResult> Index()
        {
            ViewBag.Filter = false;
            var list = await _restaurant.GetUserRestaurantList(0); //default userId 0

            var model = new AnalyticViewModel();

            model.RestaurantId = _obj.Analytic_RestaurantId;
            model.RobotId = _obj.Analytic_RobotId;

            model.Restaurants = new List<SelectListItem>();
            model.Restaurants.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0"
            });

            foreach(var data in list)
            {
                model.Restaurants.Add(new SelectListItem()
                {
                    Text = data.restaurant_name,
                    Value = data.restaurant_id.ToString()
                });
            }

            var robotlist = await _restaurant.GetRobotListByRestId(model.RestaurantId);
            model.Robots = new List<SelectListItem>();
            model.Robots.Add(new SelectListItem() 
            { 
                Text = "All", 
                Value = "0"
            });

            foreach(var data in robotlist)
            {
                model.Robots.Add(new SelectListItem()
                {
                    Text = data.Name,
                    Value = data.Id.ToString()
                });
            }
           

            if(_obj.Analytic_start.Date == DateTime.MinValue)
            {
                _obj.Analytic_start = DateTime.Now;
                _obj.Analytic_end = DateTime.Now;
            }

            model.StartDate = _obj.Analytic_start.ToString("dd/MM/yyyy");
            model.EndDate = _obj.Analytic_end.ToString("dd/MM/yyyy");
            model.TotalCompletedOrders = await GetTotalCompletedOrder(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            model.TotalServedRequests = await GetTotalServedRequest(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            model.AverageOrderRequestTime = await GetAverageOrderRequestTime(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            await GetCustomOrdersBar();
            await GetTodayOrdersBar();
            await GetWeeklyOrdersBar();
            await GetMonthlyOrdersBar();
            await GetYearlyOrdersBar();

            await GetCustomRequestsBar();
            await GetTodayRequestsBar();
            await GetWeeklyRequestsBar();
            await GetMonthlyRequestsBar();
            await GetYearlyRequestsBar();

            await GetCustomAvgOrderRequestDurationBar();
            await GetTodayAvgOrderRequestDurationBar();
            await GetWeeklyAvgOrderRequestDurationBar();
            await GetMonthlyAvgOrderRequestDurationBar();
            await GetYearlyAvgOrderRequestDurationBar();

            return View(model);
        }

        public async Task<int> GetTotalCompletedOrder(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            var result = await _analytic.GetTotalCompletedOrders(startDate,endDate,restaurantId, robotId);

            return result;
        }

        public async Task<int> GetTotalServedRequest(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            var result = await _analytic.GetTotalServedRequests(startDate, endDate, restaurantId, robotId);

            return result;
        }

        public async Task<int> GetAverageOrderRequestTime(DateTime startDate, DateTime endDate, int restaurantId, int robotId)
        {
            var result = await _analytic.GetAverageOrderRequestTime(startDate, endDate, restaurantId, robotId);

            return Convert.ToInt32(result);
        }


        public async Task GetCustomOrdersBar()
        {
            var result = await _analytic.GetCustomOrdersBarChart(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var orderBarLabel = new List<string>();
            var orderBarValue = new List<string>();

            foreach (var data in result)
            {
                orderBarLabel.Add(data.BarLabel);
                orderBarValue.Add(data.BarData);
            }

            ViewBag.CustomOrderBarLabel = orderBarLabel;
            ViewBag.CustomOrderBarValue = orderBarValue;
        }

        public async Task GetTodayOrdersBar()
        {
            var result = await _analytic.GetTodayOrdersBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var orderBarLabel = new List<string>();
            var orderBarValue = new List<string>();

            foreach(var data in result)
            {
                orderBarLabel.Add(data.BarLabel);
                orderBarValue.Add(data.BarData);

            }

            ViewBag.TodayOrderBarLabel = orderBarLabel;
            ViewBag.TodayOrderBarValue = orderBarValue;
        }

        public async Task GetWeeklyOrdersBar()
        {
            var result = await _analytic.GetWeeklyOrdersBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var orderBarLabel = new List<string>();
            var orderBarValue = new List<string>();

            foreach (var data in result)
            {
                orderBarLabel.Add(data.BarLabel);
                orderBarValue.Add(data.BarData);

            }

            ViewBag.WeeklyOrderBarLabel = orderBarLabel;
            ViewBag.WeeklyOrderBarValue = orderBarValue;
        }

        public async Task GetMonthlyOrdersBar()
        {
            var result = await _analytic.GetMonthlyOrdersBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var orderBarLabel = new List<string>();
            var orderBarValue = new List<string>();

            foreach (var data in result)
            {
                orderBarLabel.Add(data.BarLabel);
                orderBarValue.Add(data.BarData);

            }

            ViewBag.MonthlyOrderBarLabel = orderBarLabel;
            ViewBag.MonthlyOrderBarValue = orderBarValue;
        }

        public async Task GetYearlyOrdersBar()
        {
            var result = await _analytic.GetYearlyOrdersBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var orderBarLabel = new List<string>();
            var orderBarValue = new List<string>();

            foreach (var data in result)
            {
                orderBarLabel.Add(data.BarLabel);
                orderBarValue.Add(data.BarData);

            }

            ViewBag.YearlyOrderBarLabel = orderBarLabel;
            ViewBag.YearlyOrderBarValue = orderBarValue;
        }


        public async Task GetCustomRequestsBar()
        {
            var result = await _analytic.GetCustomRequestsBarChart(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestBarLabel = new List<string>();
            var requestBarValue = new List<string>();

            foreach (var data in result)
            {
                requestBarLabel.Add(data.BarLabel);
                requestBarValue.Add(data.BarData);

            }

            ViewBag.CustomRequestBarLabel = requestBarLabel;
            ViewBag.CustomRequestBarValue = requestBarValue;
        }

        public async Task GetTodayRequestsBar()
        {
            var result = await _analytic.GetTodayRequestsBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestBarLabel = new List<string>();
            var requestBarValue = new List<string>();

            foreach (var data in result)
            {
                requestBarLabel.Add(data.BarLabel);
                requestBarValue.Add(data.BarData);

            }

            ViewBag.TodayRequestBarLabel = requestBarLabel;
            ViewBag.TodayRequestBarValue = requestBarValue;
        }

        public async Task GetWeeklyRequestsBar()
        {
            var result = await _analytic.GetWeeklyRequestsBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestBarLabel = new List<string>();
            var requestBarValue = new List<string>();

            foreach (var data in result)
            {
                requestBarLabel.Add(data.BarLabel);
                requestBarValue.Add(data.BarData);

            }

            ViewBag.WeeklyRequestBarLabel = requestBarLabel;
            ViewBag.WeeklyRequestBarValue = requestBarValue;
        }

        public async Task GetMonthlyRequestsBar()
        {
            var result = await _analytic.GetMonthlyRequestsBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestBarLabel = new List<string>();
            var requestBarValue = new List<string>();

            foreach (var data in result)
            {
                requestBarLabel.Add(data.BarLabel);
                requestBarValue.Add(data.BarData);

            }

            ViewBag.MonthlyRequestBarLabel = requestBarLabel;
            ViewBag.MonthlyRequestBarValue = requestBarValue;
        }

        public async Task GetYearlyRequestsBar()
        {
            var result = await _analytic.GetYearlyRequestsBarChart(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestBarLabel = new List<string>();
            var requestBarValue = new List<string>();

            foreach (var data in result)
            {
                requestBarLabel.Add(data.BarLabel);
                requestBarValue.Add(data.BarData);

            }

            ViewBag.YearlyRequestBarLabel = requestBarLabel;
            ViewBag.YearlyRequestBarValue = requestBarValue;
        }


        public async Task GetCustomAvgOrderRequestDurationBar()
        {
            var orderResult = await _analytic.GetCustomAvgOrderTime(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestResult = await _analytic.GetCustomAvgRequestTime(_obj.Analytic_start, _obj.Analytic_end, _obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var customDurationLabel = new List<string>();
            var orderDurationBarValue = new List<string>();
            var requestDurationBarValue = new List<string>();

            foreach (var orderData in orderResult)
            {
                customDurationLabel.Add(orderData.BarLabel);
                orderDurationBarValue.Add(orderData.BarData);
            }

            foreach (var requestData in requestResult)
            {
                if (!customDurationLabel.Contains(requestData.BarLabel))
                {
                    customDurationLabel.Add(requestData.BarLabel);
                    orderDurationBarValue.Add("0");
                }
                requestDurationBarValue.Add(requestData.BarData);
            }

            ViewBag.CustomDurationLabel = customDurationLabel;
            ViewBag.CustomOrderDurationBarValue = orderDurationBarValue;
            ViewBag.CustomRequestDurationBarValue = requestDurationBarValue;
        }

        public async Task GetTodayAvgOrderRequestDurationBar()
        {
            var orderResult = await _analytic.GetTodayAvgOrderTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestResult = await _analytic.GetTodayAvgRequestTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var todayDurationLabel = new List<string>();
            var orderDurationBarValue = new List<string>();
            var requestDurationBarValue = new List<string>();

            foreach (var orderData in orderResult)
            {
                todayDurationLabel.Add(orderData.BarLabel);
                orderDurationBarValue.Add(orderData.BarData);
            }

            foreach (var requestData in requestResult)
            {
                if (!todayDurationLabel.Contains(requestData.BarLabel))
                {
                    todayDurationLabel.Add(requestData.BarLabel);
                    orderDurationBarValue.Add("0"); 
                }
                requestDurationBarValue.Add(requestData.BarData);
            }

            ViewBag.TodayDurationLabel = todayDurationLabel;
            ViewBag.TodayOrderDurationBarValue = orderDurationBarValue;
            ViewBag.TodayRequestDurationBarValue = requestDurationBarValue;
        }

        public async Task GetWeeklyAvgOrderRequestDurationBar()
        {
            var orderResult = await _analytic.GetWeeklyAvgOrderTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestResult = await _analytic.GetWeeklyAvgRequestTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var weeklyDurationLabel = new List<string>();
            var orderDurationBarValue = new List<string>();
            var requestDurationBarValue = new List<string>();

            foreach (var orderData in orderResult)
            {
                weeklyDurationLabel.Add(orderData.BarLabel);
                orderDurationBarValue.Add(orderData.BarData);
            }

            foreach (var requestData in requestResult)
            {
                if (!weeklyDurationLabel.Contains(requestData.BarLabel))
                {
                    weeklyDurationLabel.Add(requestData.BarLabel);
                    orderDurationBarValue.Add("0");
                }
                requestDurationBarValue.Add(requestData.BarData);
            }

            ViewBag.WeeklyDurationLabel = weeklyDurationLabel;
            ViewBag.WeeklyOrderDurationBarValue = orderDurationBarValue;
            ViewBag.WeeklyRequestDurationBarValue = requestDurationBarValue;
        }

        public async Task GetMonthlyAvgOrderRequestDurationBar()
        {
            var orderResult = await _analytic.GetMonthlyAvgOrderTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestResult = await _analytic.GetMonthlyAvgRequestTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var monthlyDurationLabel = new List<string>();
            var orderDurationBarValue = new List<string>();
            var requestDurationBarValue = new List<string>();

            foreach (var orderData in orderResult)
            {
                monthlyDurationLabel.Add(orderData.BarLabel);
                orderDurationBarValue.Add(orderData.BarData);
            }

            foreach (var requestData in requestResult)
            {
                if (!monthlyDurationLabel.Contains(requestData.BarLabel))
                {
                    monthlyDurationLabel.Add(requestData.BarLabel);
                    orderDurationBarValue.Add("0"); 
                }
                requestDurationBarValue.Add(requestData.BarData);
            }

            ViewBag.MonthlyDurationLabel = monthlyDurationLabel;
            ViewBag.MonthlyOrderDurationBarValue = orderDurationBarValue;
            ViewBag.MonthlyRequestDurationBarValue = requestDurationBarValue;
        }

        public async Task GetYearlyAvgOrderRequestDurationBar()
        {
            var orderResult = await _analytic.GetYearlyAvgOrderTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);
            var requestResult = await _analytic.GetYearlyAvgRequestTime(_obj.Analytic_RestaurantId, _obj.Analytic_RobotId);

            var yearlyDurationLabel = new List<string>();
            var orderDurationBarValue = new List<string>();
            var requestDurationBarValue = new List<string>();

            foreach (var orderData in orderResult)
            {
                yearlyDurationLabel.Add(orderData.BarLabel);
                orderDurationBarValue.Add(orderData.BarData);
            }

            foreach (var requestData in requestResult)
            {
                if (!yearlyDurationLabel.Contains(requestData.BarLabel))
                {
                    yearlyDurationLabel.Add(requestData.BarLabel);
                    orderDurationBarValue.Add("0"); 
                }
                requestDurationBarValue.Add(requestData.BarData);
            }

            ViewBag.YearlyDurationLabel = yearlyDurationLabel;
            ViewBag.YearlyOrderDurationBarValue = orderDurationBarValue;
            ViewBag.YearlyRequestDurationBarValue = requestDurationBarValue;
        }



        [HttpGet]
        public async Task<JsonInfoResult> RestaurantChange(int id)
        {
            var info = new JsonInfoResult();

            _obj.Analytic_RestaurantId = id;
            _obj.Analytic_RobotId = 0;

            return info;
        }

        [HttpGet]
        public async Task<JsonInfoResult> RobotChange(int id)
        {
            var info = new JsonInfoResult();

            _obj.Analytic_RobotId = id;

            return info;
        }

        [HttpGet]
        public async Task<JsonInfoResult> StartChange(string dates)
        {
            var info = new JsonInfoResult();

            DateTime start = DateTime.ParseExact(dates, "dd/MM/yyyy", null);

            _obj.Analytic_start = start;

            return info;
        }

        [HttpGet]
        public async Task<JsonInfoResult> EndChange(string dates)
        {
            var info = new JsonInfoResult();

            DateTime end = DateTime.ParseExact(dates, "dd/MM/yyyy", null);

            _obj.Analytic_end = end;

            return info;
        }

    }
}