using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class UpdateBedFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long BedPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string LockerId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int Priority { get; set; }
    }
}
