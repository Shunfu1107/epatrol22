using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class UpdateRoleFilter : BaseFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long RolePvid { get; set; }

        [ParameterizeQuery(DbType = SqlDbType.NVarChar)]
        public string Name { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm")]
        public DateTime StartActiveDate { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.DateTime, Format = "yyyy/MM/dd HH:mm")]
        public DateTime EndActiveDate { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Bit)]
        public bool IsActive { get; set; }

        public static implicit operator UpdateRoleFilter(AppRole role)
        {
            return new UpdateRoleFilter
            {
                RolePvid = role.Pvid,
                EndActiveDate = role.EndActiveDate,
                StartActiveDate = role.StartActiveDate,
                Name = role.Name,
                IsActive = role.IsActive
            };
        }
    }
}
