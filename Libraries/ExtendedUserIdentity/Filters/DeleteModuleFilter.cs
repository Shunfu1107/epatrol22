using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteModuleFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }

        public static implicit operator DeleteModuleFilter(AppModule module)
        {
            return new DeleteModuleFilter
            {
                ModulePvid = module.Pvid
            };
        }
    }
}
