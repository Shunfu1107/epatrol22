using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models
{
    public class ContentPermission : IContent
    {
        public string Key { get; set; }
        public string Desc { get; set; }
        public string Title { get; set; }
        public string AssociatedKey { get; set; }
        public string DateCreated { get; set; }
        public bool StaticAuthorization { get; set; }
    }
}