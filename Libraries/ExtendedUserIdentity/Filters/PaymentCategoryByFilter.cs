using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class PaymentCategoryByFilter
    {
        [ParameterizeQuery(DbType = SqlDbType.Int)]
        public long PaymentCategoryID { get; set; }

        public static implicit operator PaymentCategoryByFilter(long paymentCategoryId)
        {
            return new PaymentCategoryByFilter
            {
                PaymentCategoryID = paymentCategoryId
            };
        }
    }
}
