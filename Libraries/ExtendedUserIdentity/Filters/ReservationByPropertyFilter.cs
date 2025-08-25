using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class ReservationByPropertyFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long PropertyPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy-MM-dd")]
        public DateTime StartDate { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy-MM-dd")]
        public DateTime EndDate { get; set; }
    }
}
