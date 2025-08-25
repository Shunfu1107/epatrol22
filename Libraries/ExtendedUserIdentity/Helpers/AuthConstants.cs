using AdminPortalV8.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public class AuthConstants
    {
        public const string TBL_USER = "dbo.[User]";
        public const string TBL_PERSON = "dbo.Persons";
        public const string TBL_USERPROFILES = "dbo.IdentityUserProfiles";
        public const string TBL_ROLEPROFILES = "dbo.IdentityRoleProfiles";
        public const string TBL_ROLE = "dbo.IdentityRole";
        public const string TBL_ROLE_ACCESS = "dbo.IdentityRoleAccess";
        public const string TBL_ACCESS_LEVEL = "dbo.IdentityAccessLevel";
        public const string TBL_ACCESS_LEVEL_DETAIL = "dbo.IdentityAccessLevelDetail";
        public const string TBL_ACCESS_LEVEL_EXCLUSIVE = "dbo.IdentityAccessLevelExclusive";
        public const string TBL_MODULE = "dbo.IdentityModule";
        public const string TBL_MODULE_ORGANIZE = "dbo.IdentityModuleOrganize";
        public const string TBL_USERPASSWORDHISTORY = "dbo.UserPasswordHistory";

        public const string TBL_PERMISSION = "dbo.IdentityPermission";
        public const string TBL_USERPERMISSION = "dbo.IdentityUserPermission";

        

        public const string DefaultUsername = "Admin007";
        public const string DefaultPassword = "P@ss_w0rd";
        public const string DefaultEmail = "support@mquestsys.com";
        public const string DefaultAccessLevelName = "Admin";
        public const string DefaultModuleName = "Administration";
        public const string DefaultRoleName = "Admin";
        public const string DefaultLocationName = "MQuest";

        
    }

  
}