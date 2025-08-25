using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database
{
    public class DatabaseConfiguration
    {
        private static string connString = "";

        public static string ConnectionString
        {
            get
            {
                if (connString == "")
                {
                    connString = GetConnectionString();
                }
                return connString;
            }
            set
            {
                connString = value;
            }
        }


        public static string GetConnectionString()
        {
            try
            {
                return "Data Source=202.73.50.123;Initial Catalog=RTLSCloud;User ID=sa;Password=mquest0509;Encrypt=False";

            }
            catch
            {
                return "";
            }
        }
    }
}