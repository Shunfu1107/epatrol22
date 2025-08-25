using System;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public sealed class RoomTypeAvailabilityFilter
    {
        public string PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
