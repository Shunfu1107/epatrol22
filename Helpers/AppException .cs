namespace AdminPortalV8.Helpers
{
    //Custom exception for services
    public class AppException : Exception
    {
        public string Key { get; set; }
        public Dictionary<string, string> Exceptions { get; set; }
        private AppException() : base() { }

        public AppException(string key, string message) : base(message)
        {
            Key = key;
        }



        public AppException(Dictionary<string, string> ex)
        {
            Exceptions = ex;
        }

        public AppException(string message) : base(message) { Key = "Undefined"; }
    }
}
