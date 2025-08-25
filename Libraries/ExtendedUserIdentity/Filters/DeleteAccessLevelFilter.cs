using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class DeleteAccessLevelFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long AccessLevelPvid { get; set; }

        public static implicit operator DeleteAccessLevelFilter(AppAccessLevel accessLevel)
        {
            return new DeleteAccessLevelFilter
            {

                AccessLevelPvid = accessLevel.Pvid
            };
        }
    }
}
