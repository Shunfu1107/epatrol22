using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class GuestCardFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long PropertyID { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd")]
        public DateTime Date { get; set; }
    }
}
