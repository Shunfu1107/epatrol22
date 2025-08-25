using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;


namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateAccessLevelFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long AccessLevelPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }

        public static implicit operator UpdateAccessLevelFilter(AppAccessLevel access)
        {
            return new UpdateAccessLevelFilter
            {
                AccessLevelPvid = access.Pvid,
                Name = access.Name
            };
        }
    }
}
