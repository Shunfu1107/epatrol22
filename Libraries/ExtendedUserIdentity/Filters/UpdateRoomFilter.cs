using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateRoomFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RoomPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RoomTypePvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long CompanyPvid { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int Priority { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int FloorLevel { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string DoorId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }
    }
}
