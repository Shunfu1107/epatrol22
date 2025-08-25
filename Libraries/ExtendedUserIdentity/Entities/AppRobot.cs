using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppRobot
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long robot_id { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public string robot_serialnum { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public DateTime created_datetime { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm")]
        public string PermissionKey { get; set; }
    }
}
