using System.Collections.Generic;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models
{
    public class Paging<T>
    {
        public Paging()
        {
            PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }
        public IList<T> List { get; set; }
    }
}