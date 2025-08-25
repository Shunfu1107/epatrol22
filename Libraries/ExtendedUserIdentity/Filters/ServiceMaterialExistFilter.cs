using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class ServiceMaterialExistFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long MaterialId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long PaymentCategoryId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.BigInt)]
        public long PaymentSubCategoryId { get; set; }
    }
}
