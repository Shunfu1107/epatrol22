using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class ModuleByNameFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string ModuleName { get; set; }

        public static implicit operator ModuleByNameFilter(string moduleName)
        {
            return new ModuleByNameFilter
            {
                ModuleName = moduleName
            };
        }
        public static implicit operator ModuleByNameFilter(AppModule module)
        {
            return module.Name;
        }
    }
}
