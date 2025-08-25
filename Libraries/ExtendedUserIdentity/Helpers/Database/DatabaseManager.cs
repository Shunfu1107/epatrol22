using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database
{
    [DebuggerNonUserCode]
    public class DatabaseManager
    {
        protected string ConnectionString
        {
            get
            {
                return DatabaseConfiguration.GetConnectionString();
            }
        }

        internal SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        protected SqlCommand CreateSqlCommand(string query)
        {
            return new SqlCommand(query, CreateSqlConnection());
        }

        protected SqlCommand CreateSqlCommand(SQLBaseCommand query)
        {
            var cmd = new SqlCommand(query, CreateSqlConnection());
            cmd.CommandType = query.CommandType;
            cmd.Parameters.AddRange(query.GetParameters());
            return cmd;
        }
    }
}