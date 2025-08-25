namespace AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers
{
    public static partial class AppModuleKeys
    {
        public class PortalAdmin
        {
            public const string ViewPrimaryKey = nameof(PortalAdmin) + ".View";
            public const string ViewTitle = "Users";
            public const string ViewDescription = "Manage(add/edit/delete/view) Users";
            public const bool ViewStaticAuthorized = true;

            public const string EditPrimaryKey = nameof(PortalAdmin) + ".Edit";
            public const string EditTitle = "Users";
            public const string EditDescription = "Edit User";
            public const bool EditStaticAuthorized = true;

            public const string RemovePrimaryKey = nameof(PortalAdmin) + ".Remove";
            public const string RemoveTitle = "Users";
            public const string RemoveDescription = "Remove User";
            public const bool RemoveStaticAuthorized = true;

            public const string AddPrimaryKey = nameof(PortalAdmin) + ".Add";
            public const string AddTitle = "Users";
            public const string AddDescription = "Add User";
            public const bool AddStaticAuthorized = true;

            public const string ManageAccessRightsPrimaryKey = nameof(PortalAdmin) + ".ManageAccessRights";
            public const string ManageAccessRightsTitle = "Users";
            public const string ManageAccessRightsDescription = "Manage User Access Rights";
            public const bool ManageAccessRightsStaticAuthorized = true;

            public const string AddRolePrimaryKey = nameof(PortalAdmin) + ".AddRole";
            public const string AddRoleTitle = "Users";
            public const string AddRoleDescription = "Add User Role";
            public const bool AddRoleStaticAuthorized = true;

            public const string RemoveRolePrimaryKey = nameof(PortalAdmin) + ".RemoveRole";
            public const string RemoveRoleTitle = "Users";
            public const string RemoveRoleDescription = "Remove User Role";
            public const bool RemoveRoleStaticAuthorized = true;

            public const string AddExclusiveAccessRightsPrimaryKey = nameof(PortalAdmin) + ".AddExclusiveAccessRights";
            public const string AddExclusiveAccessRightsTitle = "Users";
            public const string AddExclusiveAccessRightsDescription = "Add User Exclusive Access Rights";
            public const bool AddExclusiveAccessRightsStaticAuthorized = true;

            public const string EditExclusiveAccessRightsPrimaryKey = nameof(PortalAdmin) + ".EditExclusiveAccessRights";
            public const string EditExclusiveAccessRightsTitle = "Users";
            public const string EditExclusiveAccessRightsDescription = "Edit User Exclusive Access Rights";
            public const bool EditExclusiveAccessRightsStaticAuthorized = true;

            public const string RemoveExclusiveAccessRightsPrimaryKey = nameof(PortalAdmin) + ".RemoveExclusiveAccessRights";
            public const string RemoveExclusiveAccessRightsTitle = "Users";
            public const string RemoveExclusiveAccessRightsDescription = "Remove User Exclusive Access Rights";
            public const bool RemoveExclusiveAccessRightsStaticAuthorized = true;
        }

        public static class Module
        {
            public const string ViewPrimaryKey = nameof(Module) + ".View";
            public const string ViewTitle = "Module";
            public const string ViewDescription = "Manage(add/edit/delete/view) Modules";
            public const bool ViewStaticAuthorized = true;

            public const string EditPrimaryKey = nameof(Module) + ".Edit";
            public const string EditTitle = "Module";
            public const string EditDescription = "Edit Module";
            public const bool EditStaticAuthorized = true;

            public const string RemovePrimaryKey = nameof(Module) + ".Remove";
            public const string RemoveTitle = "Module";
            public const string RemoveDescription = "Remove Module";
            public const bool RemoveStaticAuthorized = true;

            public const string AddPrimaryKey = nameof(Module) + ".Add";
            public const string AddTitle = "Module";
            public const string AddDescription = "Add Module";
            public const bool AddStaticAuthorized = true;

            public const string ManagePrimaryKey = nameof(Module) + ".Manage";
            public const string ManageTitle = "Module";
            public const string ManageDescription = "Manage Modules";
            public const bool ManageStaticAuthorized = true;

            public const string ManagePermissionPrimaryKey = nameof(Module) + ".ManagePermissions";
            public const string ManagePermissionTitle = "Module";
            public const string ManagePermissionDescription = "Manage Permissions";
            public const bool ManagePermissionStaticAuthorized = true;

        }

        public static class AccessLevel
        {
            public const string ViewPrimaryKey = nameof(AccessLevel) + ".View";
            public const string ViewTitle = "Access Level";
            public const string ViewDescription = "Manage(add/edit/delete/view) Access Level";
            public const bool ViewStaticAuthorized = true;

            public const string EditPrimaryKey = nameof(AccessLevel) + ".Edit";
            public const string EditTitle = "Access Level";
            public const string EditDescription = "Edit Access Level";
            public const bool EditStaticAuthorized = true;

            public const string RemovePrimaryKey = nameof(AccessLevel) + ".Remove";
            public const string RemoveTitle = "Access Level";
            public const string RemoveDescription = "Remove Access Level";
            public const bool RemoveStaticAuthorized = true;

            public const string AddPrimaryKey = nameof(AccessLevel) + ".Add";
            public const string AddTitle = "Access Level";
            public const string AddDescription = "Add Access Level";
            public const bool AddStaticAuthorized = true;

            public const string ManagePrimaryKey = nameof(AccessLevel) + ".Manage";
            public const string ManageTitle = "Access Level";
            public const string ManageDescription = "Manage Access Levels";
            public const bool ManageStaticAuthorized = true;

            public const string ManagePermissionPrimaryKey = nameof(AccessLevel) + ".ManagePermissions";
            public const string ManagePermissionTitle = "Access Level";
            public const string ManagePermissionDescription = "Manage Permissions";
            public const bool ManagePermissionStaticAuthorized = true;

        }

        public static class Role
        {
            public const string ViewPrimaryKey = nameof(Role) + ".View";
            public const string ViewTitle = "Role";
            public const string ViewDescription = "Manage(add/edit/delete/view) Roles";
            public const bool ViewStaticAuthorized = true;

            public const string EditPrimaryKey = nameof(Role) + ".Edit";
            public const string EditTitle = "Role";
            public const string EditDescription = "Edit Role";
            public const bool EditStaticAuthorized = true;

            public const string RemovePrimaryKey = nameof(Role) + ".Remove";
            public const string RemoveTitle = "Role";
            public const string RemoveDescription = "Remove Role";
            public const bool RemoveStaticAuthorized = true;

            public const string AddPrimaryKey = nameof(Role) + ".Add";
            public const string AddTitle = "Role";
            public const string AddDescription = "Add Role";
            public const bool AddStaticAuthorized = true;

            public const string ManagePrimaryKey = nameof(Role) + ".Manage";
            public const string ManageTitle = "Role";
            public const string ManageDescription = "Manage Roles";
            public const bool ManageStaticAuthorized = true;

            public const string AddRoleAccessPrimaryKey = nameof(Role) + ".AddRoleAccess";
            public const string AddRoleAccessTitle = "Access Level";
            public const string AddRoleAccessDescription = "Add Role Access";
            public const bool AddRoleAccessStaticAuthorized = true;

            public const string RemoveRoleAccessPrimaryKey = nameof(Role) + ".RemoveRoleAccess";
            public const string RemoveRoleAccessTitle = "Access Level";
            public const string RemoveRoleAccessDescription = "Remove Role Access";
            public const bool RemoveRoleAccessStaticAuthorized = true;

            public const string AddOwnershipAccessPrimaryKey = nameof(Role) + ".AddOwnership";
            public const string AddOwnershipAccessTitle = "Access Level";
            public const string AddOwnershipAccessDescription = "Add Ownership";
            public const bool AddOwnershipAccessStaticAuthorized = true;

            public const string RemoveOwnershipAccessPrimaryKey = nameof(Role) + ".RemoveOwnership";
            public const string RemoveOwnershipAccessTitle = "Access Level";
            public const string RemoveOwnershipAccessDescription = "Remove Ownership";
            public const bool RemoveOwnershipAccessStaticAuthorized = true;
        }

        public static class RoleLevel
        {
            public const string SystemPrimaryKey = "System.access";
            public const string SystemTitle = "System Admins";
            public const string SystemDescription = "Check this if this role is System Admin";
            public const bool SystemStaticAuthorized = true;

            public const string AdminPrimaryKey = "Admin.access";
            public const string AdminTitle = "Admin Admins";
            public const string AdminDescription = "Check this if this role is Admin";
            public const bool AdminStaticAuthorized = true;
        }
    }
}
