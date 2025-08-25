using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppRole
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string? Name { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm")]
        public DateTime StartActiveDate { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm")]
        public DateTime EndActiveDate { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Bit)]
        public bool IsActive { get; set; }

        public IList<AppRoleAccess> AccessList { get; set; }
        public string? username { get; set; }
    }
}
