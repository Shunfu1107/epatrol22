namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ManageModulePermissionViewModel
    {
        public long ModulePvid { get; set; }
        public string PermissionKey { get; set; }
        public bool Included { get; set; }
    }
}