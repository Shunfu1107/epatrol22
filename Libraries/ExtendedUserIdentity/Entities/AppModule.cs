using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Newtonsoft.Json;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Entities
{
    public class AppModule
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt, IsRequired = false)]
        public long Pvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public IList<AppModuleOrganize>? Permissions { get; set; }
    }
}
