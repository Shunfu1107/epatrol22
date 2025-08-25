public class DashboardDataWrapper
{
    public List<int> WeeklyOrdersCount { get; set; }
    public List<int> YearlyOrdersCount { get; set; }
    public List<int> YearlyRequestsCount { get; set; }
    public int TotalRestaurants { get; set; }
    public int TotalRobots { get; set; }

    public DashboardDataWrapper()
    {
        WeeklyOrdersCount = new List<int>();
        YearlyOrdersCount = new List<int>();
        YearlyRequestsCount = new List<int>();
        TotalRestaurants = 0;
        TotalRobots = 0;
    }
}



//    public class DashboardDataWrapper
//    {
//        public List<int> OrdersByWeek { get; set; }
//        public List<object> OrdersAndRequestsByYear { get; set; }
//        public int TotalRestaurants { get; set; }
//        public int TotalRobots { get; set; }

//        public DashboardDataWrapper()
//        {
//            OrdersByWeek = new List<int>();
//            OrdersAndRequestsByYear = new List<object>();
//            TotalRestaurants = 0;
//            TotalRobots = 0;
//        }
//    }

//    public class OrderModel
//    {
//        public string? Day {  get; set; }
//        public int? Count {  get; set; }

//    }

//    public class OrderRequestModel
//    {
//        public int? OrdersCount { get; set; }
//        public int? RequestsCount { get; set; }
//        public string? Month { get; set; }
//    }

