using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class ModuleByIdFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long ModulePvid { get; set; }

        public static implicit operator ModuleByIdFilter(long modulePvid)
        {
            return new ModuleByIdFilter
            {
                ModulePvid = modulePvid
            };
        }
    }
}
