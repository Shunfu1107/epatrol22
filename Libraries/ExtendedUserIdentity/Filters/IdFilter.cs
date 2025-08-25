using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class IdFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long Pvid { get; set; }

        public static implicit operator IdFilter(long pvid)
        {
            return new IdFilter
            {
                Pvid = pvid
            };
        }
    }
}
