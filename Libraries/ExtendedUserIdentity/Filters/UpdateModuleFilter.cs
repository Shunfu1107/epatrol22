using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateModuleFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public static implicit operator UpdateModuleFilter(AppModule module)
        {
            return new UpdateModuleFilter
            {
                ModulePvid = module.Pvid,
                Name = module.Name
            };
        }
    }
}
