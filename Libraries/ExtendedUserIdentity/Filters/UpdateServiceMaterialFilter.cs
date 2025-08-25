using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class UpdateServiceMaterialFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int ServiceId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int PaymentCategoryId { get; set; }
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public int PaymentSubCategoryId { get; set; }
    }
}
