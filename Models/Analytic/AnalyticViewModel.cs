using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdminPortalV8.Entities;
using AdminPortalV8.Models.Restaurant;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AdminPortalV8.Models.Analytic
{
    public class AnalyticViewModel
    {
        [Display(Name = "Date")]
        public string StartDate { get; set; }
        [Display(Name = "To")]
        public string EndDate { get; set; }
        [Display(Name = "Restaurant")]
        public int RestaurantId { get; set; }
        [Display(Name = "Robot")]
        public int RobotId { get; set; }
        public int Year { get; set; } 
        public int Month { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalServedRequests { get; set; }
        public double AverageOrderRequestTime { get; set; }
        public List<SelectListItem> Restaurants { get; set; }
        public List<SelectListItem> Robots { get; set; }

    }

    public class BarChartViewModel
    {
        public string BarLabel { get; set; }
        public string BarData { get; set; }
    }
}
