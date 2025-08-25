namespace AdminPortalV8.Models.User
{
    public class PermissionData
    {
        public string? Key { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool StaticAuthorized { get; set; }
        public string? AssociatedKey { get; set; }
    }
}
