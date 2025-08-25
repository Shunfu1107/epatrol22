using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public class DataFill
    {
        public void fillUser(DataRow reader, out AppUser user)
        {
            user = new AppUser();
            var obj = default(object);

            obj = reader["USRPvid"];
            user.UserID = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["USRUsername"];
            user.UserName = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USRSecurityStamp"];
            user.SecurityStamp = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USRPasswordHash"];
            user.Password = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USRName"];
            user.Name = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USREmail"];
            user.Email = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USRActive"];
            user.Active = obj == DBNull.Value ? true : Convert.ToBoolean(obj);

            obj = reader["USRReceivedEmail"];
            user.ReceivedEmail = obj == DBNull.Value ? "No" : Convert.ToBoolean(obj) == true ? "Yes" : "No";

            //if (reader.ItemArray.Length > 7)
            //{
            //    obj = reader["NRIC"];
            //    user.NRIC = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            //    obj = reader["Mobile"];
            //    user.Mobile = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);
            //}
        }
        public void fillUserProfiles(DataRow reader, out AppUserProfiles userProfiles)
        {
            userProfiles = new AppUserProfiles();
            var obj = default(object);

            obj = reader["USRPROPvid"];
            userProfiles.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (userProfiles.Pvid.Equals(0)) return;

            obj = reader["USRPROIdentityUserPvid"];
            userProfiles.UserPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);


            userProfiles.AccessLevelExclusiveList = new List<AppAccessLevelExclusive>(3);
            userProfiles.RoleProfiles = new List<AppRoleProfiles>(3);
        }
        public void fillUserPermission(DataRow reader, out AppAccessLevelExclusive userPermission)
        {
            userPermission = new AppAccessLevelExclusive();
            var obj = default(object);

            obj = reader["USERPERMPvid"];
            userPermission.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (userPermission.Pvid.Equals(0)) return;

            obj = reader["USERPERMIdentityUserPvid"];
            userPermission.IdentityUserPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["USERPERMPermissionKey"];
            userPermission.PermissionKey = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["USERPERMAccessible"];
            userPermission.Accessible = obj == DBNull.Value ? false : Convert.ToBoolean(obj);
        }
        public void fillPermission(DataRow reader, out AppPermission l)
        {
            l = new AppPermission();
            var obj = default(object);

            obj = reader["PERMPvid"];
            l.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (l.Pvid.Equals(0)) return;

            obj = reader["PERMIdentityRolePvid"];
            l.IdentityRolePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["PERMPermissionKey"];
            l.PermissionKey = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);
        }


        public void fillRole(DataRow reader, out AppRole role)
        {
            role = new AppRole();
            var obj = default(object);

            obj = reader["ROLPvid"];
            role.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (role.Pvid.Equals(0)) return;

            obj = reader["ROLName"];
            role.Name = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["ROLStartActiveDate"];
            role.StartActiveDate = obj == DBNull.Value ? new DateTime(2000, 1, 1) : Convert.ToDateTime(obj);

            obj = reader["ROLEndActiveDate"];
            role.EndActiveDate = obj == DBNull.Value ? new DateTime(2000, 1, 1) : Convert.ToDateTime(obj);

            obj = reader["ROLIsActive"];
            role.IsActive = obj == DBNull.Value ? false : Convert.ToBoolean(obj);

            role.AccessList = new List<AppRoleAccess>(1);
        }
        public void fillRoleAccess(DataRow reader, out AppRoleAccess access)
        {
            access = new AppRoleAccess();
            var obj = default(object);

            obj = reader["ROLACPvid"];
            access.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (access.Pvid.Equals(0)) return;

            obj = reader["ROLACIdentityRolePvid"];
            access.IdentityRolePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ROLACIdentityModulePvid"];
            access.IdentityModulePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ROLACIdentityAccessLevelPvid"];
            access.IdentityAccessLevelPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ROLACGrantedByIdentityUserPvid"];
            access.GrantedByIdentityUserPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ROLACGrantedDate"];
            access.GrantedDate = obj == DBNull.Value ? new DateTime(2000, 1, 1) : Convert.ToDateTime(obj);
        }

        public void fillRoleProfile(DataRow reader, out AppRoleProfiles roleprofile)
        {
            roleprofile = new AppRoleProfiles();
            var obj = default(object);

            obj = reader["ROLPROPvid"];
            roleprofile.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (roleprofile.Pvid.Equals(0)) return;

            obj = reader["ROLPROIdentityUserPvid"];
            roleprofile.IdentityUserPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ROLPROIdentityRolePvid"];
            roleprofile.IdentityRolePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
        }

        public void fillAccessLevel(DataRow reader, out AppAccessLevel access)
        {
            access = new AppAccessLevel();
            var obj = default(object);

            obj = reader["ACCPvid"];
            access.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (access.Pvid.Equals(0)) return;

            obj = reader["ACCName"];
            access.Name = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            access.AccessDetails = new List<AppAccessLevelDetail>(3);
        }
        public void fillAccessLevelDetail(DataRow reader, out AppAccessLevelDetail detail)
        {
            detail = new AppAccessLevelDetail();
            var obj = default(object);

            obj = reader["ACCDETPvid"];
            detail.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (detail.Pvid.Equals(0)) return;

            obj = reader["ACCDETIdentityAccessLevelPvid"];
            detail.IdentityAccessLevelPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ACCDETPermissionKey"];
            detail.PermissionKey = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["ACCDETIdentityModulePvid"];
            detail.IdentityModulePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
        }
        public void fillAccessLevelExclusive(DataRow reader, out AppAccessLevelExclusive exlcusive)
        {
            exlcusive = new AppAccessLevelExclusive();
            var obj = default(object);

            obj = reader["ACCEXPvid"];
            exlcusive.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            if (exlcusive.Pvid.Equals(0)) return;

            obj = reader["ACCEXIdentityUserPvid"];
            exlcusive.IdentityUserPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["ACCEXPermissionKey"];
            exlcusive.PermissionKey = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

            obj = reader["ACCEXAccessible"];
            exlcusive.Accessible = obj == DBNull.Value ? false : Convert.ToBoolean(obj);
        }

        public void fillModule(DataRow reader, out AppModule module)
        {
            module = new AppModule();
            module.Permissions = new List<AppModuleOrganize>(3);
            var obj = default(object);

            obj = reader["MODPvid"];
            module.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["MODName"];
            module.Name = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);

        }
        public void fillModuleOrganize(DataRow reader, out AppModuleOrganize moduleorganize)
        {
            moduleorganize = new AppModuleOrganize();
            var obj = default(object);

            obj = reader["MODORPvid"];
            moduleorganize.Pvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["MODORIdentityModulePvid"];
            moduleorganize.IdentityModulePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);

            obj = reader["MODORPermissionKey"];
            moduleorganize.PermissionKey = obj == DBNull.Value ? string.Empty : Convert.ToString(obj);
        }

    }
}