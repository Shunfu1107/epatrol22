using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using AdminPortalV8.Helpers;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text.Json;
using System.Security.Claims;
using AdminPortalV8.Data.ExtendedIdentity;
using Microsoft.EntityFrameworkCore;
using AdminPortalV8.Data;
using Microsoft.AspNetCore.Identity;
using System.Web.Mvc;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Services
{
    public class UserService : IUser
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private SqlConnectionStringBuilder SSB;
        private string DBString;
        private DataFill dataFill;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");

            SSB = new SqlConnectionStringBuilder(_configuration.GetConnectionString("DefaultConnection"));
            DBString = SSB.InitialCatalog;

            dataFill = new DataFill();
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
            
        }

        public AppUser GetUser(string username)
        {
            AppUser user = new AppUser();

            string query = getUserByName(username);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            user.UserID = Convert.ToInt32(dr["ID"]);
                            user.UserName = dr["UserName"].ToString();
                            user.Password = dr["Password"].ToString();
                            user.SecurityStamp = dr["SecurityStamp"].ToString();
                            user.Name = dr["Name"].ToString();
                            user.Email = dr["Email"].ToString();
                            user.Active = dr["Active"] == DBNull.Value ? true : Convert.ToBoolean(dr["Active"]);
                            user.FirstPasswordReset = dr["FirstPasswordReset"] == DBNull.Value ? false : Convert.ToBoolean(dr["FirstPasswordReset"]);
                            user.ReceivedEmail = dr["ReceivedEmail"] == DBNull.Value ? "No" : Convert.ToBoolean(dr["ReceivedEmail"]) == true ? "Yes" : "No";
                        }
                    }
                }
                connection.Close();
            }
            return user;

        }

        public bool UpdateActiveStatus(int id, bool active)
        {
            string query = updateActive(Convert.ToInt32(active).ToString(), id.ToString());
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    int i = command.ExecuteNonQuery();

                    if( i >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                connection.Close();
            }
            
        }

        string updateActive(string active, string id)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;
                UPDATE {1} SET [Active] = {2} WHERE ID = {3}", DBString, AuthConstants.TBL_USER, active, id);
        }

        string getUserByName(string username)
        {
            return string.Format(@"SELECT * FROM {0} WHERE UserName='{1}'", AuthConstants.TBL_USER, username);
        }

        string getUserByID(int id)
        {
            return string.Format(@"SELECT * FROM {0} WHERE Id='{1}'", AuthConstants.TBL_USER, id);
        }

        string getAllUsers()
        {
            return string.Format(@"SELECT * FROM {0}", AuthConstants.TBL_USER);
        }

        string userProfileQueryCondition()
        {
            return String.Format(@"SELECT 
                USRPRO.Pvid USRPROPvid,USRPRO.IdentityUserPvid USRPROIdentityUserPvid
                ,ROLPRO.Pvid ROLPROPvid, ROLPRO.IdentityRolePvid ROLPROIdentityRolePvid, ROLPRO.IdentityUserPvid ROLPROIdentityUserPvid
                ,ROL.Pvid ROLPvid, ROL.Name ROLName, ROL.StartActiveDate ROLStartActiveDate, ROL.EndActiveDate ROLEndActiveDate, ROL.IsActive ROLIsActive
                ,ROLAC.Pvid ROLACPvid, ROLAC.IdentityRolePvid ROLACIdentityRolePvid, ROLAC.IdentityModulePvid ROLACIdentityModulePvid, ROLAC.IdentityAccessLevelPvid ROLACIdentityAccessLevelPvid, ROLAC.GrantedByIdentityUserPvid ROLACGrantedByIdentityUserPvid, ROLAC.GrantedDate ROLACGrantedDate
                ,MODU.Pvid MODUPvid, MODU.Name MODUName
                ,ACC.Pvid ACCPvid,ACC.Name ACCName
                ,ACCDET.Pvid ACCDETPvid,ACCDET.IdentityAccessLevelPvid ACCDETIdentityAccessLevelPvid, ACCDET.PermissionKey ACCDETPermissionKey, ACCDET.IdentityModulePvid ACCDETIdentityModulePvid
                ,'' ROLOWPvid,'' ROLOWIdentityRolePvid,'' ROLOWGrantedByIdentityUserPvid,'' ROLOWGrantedDate
                ,'' ACCEXPvid,'' ACCEXIdentityUserPvid,'' ACCEXPermissionKey,'' ACCEXAccessible

                FROM {1} USRPRO
                LEFT JOIN {2} ROLPRO ON ROLPRO.IdentityUserPvid = USRPRO.IdentityUserPvid
                JOIN {3} ROL ON ROL.Pvid = ROLPRO.IdentityRolePvid
                LEFT JOIN {4} ROLAC ON ROLAC.IdentityRolePvid = ROL.Pvid
                LEFT JOIN {5} MODU ON MODU.Pvid = ROLAC.IdentityModulePvid
                JOIN {6} ACC ON ACC.Pvid = ROLAC.IdentityAccessLevelPvid
                JOIN {7} ACCDET ON ACCDET.IdentityAccessLevelPvid = ACC.Pvid
                WHERE USRPRO.IdentityUserPvid={0}
                UNION
                SELECT 
                USRPRO.Pvid USRPROPvid,USRPRO.IdentityUserPvid USRPROIdentityUserPvid
                ,'','', ''
                ,'', '','', '',''
                ,'','', '', '', '', ''
                ,'', ''
                ,'',''
                ,'','', '',''
                ,'' ,'','',''
                ,ACCEX.Pvid ACCEXPvid,ACCEX.IdentityUserPvid ACCEXIdentityUserPvid, ACCEX.PermissionKey ACCEXPermissionKey, ACCEX.Accessible ACCEXAccessible
                FROM {8} ACCEX 
                JOIN {1} USRPRO ON USRPRO.IdentityUserPvid = ACCEX.IdentityUserPvid
                WHERE ACCEX.IdentityUserPvid = {0}
                "
            , "@UserPvid"
                 , AuthConstants.TBL_USERPROFILES
                 , AuthConstants.TBL_ROLEPROFILES
                 , AuthConstants.TBL_ROLE
            , AuthConstants.TBL_ROLE_ACCESS
            , AuthConstants.TBL_MODULE
                 , AuthConstants.TBL_ACCESS_LEVEL
                 , AuthConstants.TBL_ACCESS_LEVEL_DETAIL
                 , AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE
                 );
        }

        public AppUserProfiles GetUserProfiles(UserProfilesByUserFilter filter)
        {
            var query = new SQLQueryCommand(userProfileQueryCondition(), filter);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserPvid", filter.UserPvid);
                    using (var reader = command.ExecuteReader())
                    {
                        var userprofile = default(AppUserProfiles);
                        var roleprofile = default(AppRoleProfiles);
                        var role = default(AppRole);
                        var roleaccess = default(AppRoleAccess);
                        var accessLevel = default(AppAccessLevel);
                        var accessDetail = default(AppAccessLevelDetail);
                        var accessExclusive = default(AppAccessLevelExclusive);
                        var obj = default(object);

                        Dictionary<long, AppRoleProfiles> tempRoleProfiles = new Dictionary<long, AppRoleProfiles>(1);
                        Dictionary<long, AppRoleAccess> tempRoleAccessList = new Dictionary<long, AppRoleAccess>(1);
                        Dictionary<long, AppRole> tempRoleList = new Dictionary<long, AppRole>(1);

                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                #region FILL: User Profiles 
                                //USER PROFILES
                                if (userprofile == null || userprofile.Pvid.Equals(0))
                                {
                                    dataFill.fillUserProfiles(row, out userprofile);
                                }
                                #endregion

                                #region FILL: ROLE PROFILES , ROLE
                                //ROLE PROFILES
                                //ROLE
                                obj = row["ROLPROPvid"];
                                var roleproid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
                                if (!roleproid.Equals(0))
                                {
                                    if (!tempRoleProfiles.Keys.Contains(roleproid))
                                    {
                                        dataFill.fillRoleProfile(row, out roleprofile);
                                        dataFill.fillRole(row, out role);

                                        roleprofile.Role = role;
                                        userprofile.RoleProfiles.Add(roleprofile);

                                        tempRoleProfiles.Add(roleprofile.Pvid, roleprofile);
                                        if (!tempRoleList.Any(x => x.Key == role.Pvid))
                                        {
                                            tempRoleList.Add(role.Pvid, role);
                                        }
                                    }
                                    else roleprofile = tempRoleProfiles[roleproid];
                                }
                                #endregion

                                #region FILL: ROLE ACCESS, ACCESS LEVEL, ACCESS LEVEL DETAIL
                                //ROLE ACCESS
                                //ACCESS LEVEL
                                //ACCESS LEVEL DETAIL
                                obj = row["ROLACPvid"];
                                var roleaccPvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
                                if (!roleproid.Equals(0) && !roleaccPvid.Equals(0))
                                {
                                    if (!tempRoleAccessList.Keys.Contains(roleaccPvid))
                                    {
                                        dataFill.fillRoleAccess(row, out roleaccess);
                                        dataFill.fillAccessLevel(row, out accessLevel);
                                        roleaccess.AccessLevel = accessLevel;
                                        roleprofile.Role.AccessList.Add(roleaccess);

                                        tempRoleAccessList.Add(roleaccPvid, roleaccess);
                                    }
                                    else roleaccess = tempRoleAccessList[roleaccPvid];

                                    dataFill.fillAccessLevelDetail(row, out accessDetail);
                                    roleaccess.AccessLevel.AccessDetails.Add(accessDetail);
                                }
                                #endregion


                                #region FILL: Access Exclusive 
                                //ACCESS EXLUSIVE
                                obj = row["ACCEXPvid"];
                                var exclusivePvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
                                if (!exclusivePvid.Equals(0))
                                {
                                    dataFill.fillAccessLevelExclusive(row, out accessExclusive);
                                    userprofile.AccessLevelExclusiveList.Add(accessExclusive);
                                }
                                #endregion
                            }
                        }

                       
                        return userprofile;
                    }
                }
                connection.Close();

            }
        }

        public void AddUserProfiles(AppUserProfiles profile)
        {
            var fields = "IdentityUserPvid";
            var values = "@UserPvid";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_USERPROFILES, fields, values);
            var query = new SQLQueryCommand(cmdText, profile);
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserPvid", profile.UserPvid);
                    profile.Pvid = Convert.ToInt64(command.ExecuteScalar());
                }

                connection.Close();
            }
                    
        }

        public AppUser FindByID(FindUserByIDFilter filter)
        {
            var query = new SQLQueryCommand(userQueryCondition("USR.ID = @Pvid"), filter);
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Pvid", filter.Pvid);
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        var user = default(AppUser);

                        if (dt.Rows.Count > 0)
                        {
                            foreach(DataRow row in dt.Rows)
                            {
                                dataFill.fillUser(row, out user);
                                break;
                            }
                        }

                        return user;
                    }
                }

                connection.Close();
            }
               
            
        }

        string userQueryCondition(string condition)
        {
            var fields = "USR.ID USRPvid,USR.UserName USRUsername,USR.SecurityStamp USRSecurityStamp,USR.Password USRPasswordHash, USR.Email USREmail, USR.Name USRName, USR.Active USRActive, USR.ReceivedEmail USRReceivedEmail";
            return string.Format("SELECT {0} FROM {1} USR WHERE {2};", fields, AuthConstants.TBL_USER, condition);
        }

        public Paging<AppUser> GetUsers(DataTablePagingFilter filter)
        {
            var query = getUserList(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.sSearch);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        
                            var totalRecords = 0;
                            var user = default(AppUser);
                            var list = new List<AppUser>(3);

                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            foreach(DataRow dr in  dt.Rows)
                            {
                                if (totalRecords.Equals(0))
                                {
                                    var obj = dr["TotalRecords"];
                                    totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                                }

                                dataFill.fillUser(dr, out user);
                                list.Add(user);

                                
                            }

                            

                            var dto = new Paging<AppUser>();
                            //var info = new PagingInfo
                            //{
                            //    CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                            //    RecordPerPage = filter.iDisplayLength,
                            //    TotalRecords = totalRecords
                            //};
                            //dto.PagingInfo = info;
                            dto.List = list;

                            return dto;
                        
                        
                    }
                }
                connection.Close();

            }

            
        }

        string getUserList(string startIndex, string length, string filter)
        {
            
            //var systemAuthorized = user.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.SystemPrimaryKey);
            //var adminAuthorized = user.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.AdminPrimaryKey);

            var uu = _httpContextAccessor.HttpContext.User;

            var systemAuthorized = uu.HasClaim(m=>m.Value == AppModuleKeys.RoleLevel.SystemPrimaryKey);
            var adminAuthorized = uu.HasClaim(m => m.Value == AppModuleKeys.RoleLevel.AdminPrimaryKey);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = "%" + filter + "%";
            }
            if (systemAuthorized)
            {
                return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;
                DECLARE @endpos int
	            DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY USR.Name ASC) AS RowNumber
		
	                ,USR.ID USRPvid
	                ,USR.Name USRName
	                ,USR.UserName USRUserName
	                ,USR.Password USRPasswordHash
	                ,USR.SecurityStamp USRSecurityStamp
	                ,USR.Email USREmail
	                ,USR.Active USRActive
                    ,USR.ReceivedEmail USRReceivedEmail
		
	                FROM {3} USR WITH(NOLOCK)
                    JOIN Persons on USR.ID = Persons.UserId
	                WHERE 
		                ('{4}' <> '' AND (USR.Name LIKE '{4}' OR USR.UserName LIKE '{4}' OR USR.Email LIKE '{4}' OR USR.ReceivedEmail '{4}')) OR
		                ('{4}' = '' AND USR.ID > 0) OR
		                ('{4}' = '%' AND USR.ID > 0) OR
		                ('{4}' = '%%' AND USR.ID > 0)
                ) Q", DBString, startIndex, length, AuthConstants.TBL_USER, filter);
            }
            else if (adminAuthorized)
            {
                return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;
                DECLARE @endpos int
	            DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY USR.UserName ASC) AS RowNumber
		
	                ,USR.ID USRPvid
	                ,USR.Name USRName
	                ,USR.UserName USRUserName
	                ,USR.Password USRPasswordHash
	                ,USR.SecurityStamp USRSecurityStamp
	                ,USR.Email USREmail
	                ,USR.Active USRActive
                    ,USR.ReceivedEmail USRReceivedEmail
		
	                FROM {3} USR WITH(NOLOCK)
                    JOIN Persons on USR.ID = Persons.UserId
	                WHERE 
		                (('{4}' <> '' AND (USR.Name LIKE '{4}' OR USR.UserName LIKE '{4}' OR USR.Email LIKE '{4}' OR USR.ReceivedEmail '{4}')) OR
		                ('{4}' = '' AND USR.ID > 0) OR
		                ('{4}' = '%' AND USR.ID > 0) OR
		                ('{4}' = '%%' AND USR.ID > 0))AND 
                        not exists (SELECT *
                        FROM [dbo].IdentityRoleProfiles AS I join [dbo].IdentityRole as R on I.IdentityRolePvid = R.Pvid
                        where USR.ID = I.IdentityUserPvid AND (R.Name) = 'SYSTEM ADMIN'))
			
                ) Q
                ORDER BY Q.[USRName]", DBString, startIndex, length, AuthConstants.TBL_USER, filter);
            }
            else
            {
                return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;
                DECLARE @endpos int
	            DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY USR.UserName ASC) AS RowNumber
		
	                ,USR.ID USRPvid
	                ,USR.Name USRName
	                ,USR.UserName USRUserName
	                ,USR.Password USRPasswordHash
	                ,USR.SecurityStamp USRSecurityStamp
	                ,USR.Email USREmail
	                ,USR.Active USRActive
		            ,USR.ReceivedEmail USRReceivedEmail

	                FROM {3} USR WITH(NOLOCK)
                    JOIN Persons on USR.ID = Persons.UserId
	                WHERE 
		                (('{4}' <> '' AND (USR.Name LIKE '{4}' OR USR.UserName LIKE '{4}' OR USR.Email LIKE '{4}' OR USR.ReceivedEmail '{4}')) OR
		                ('{4}' = '' AND USR.ID > 0) OR
		                ('{4}' = '%' AND USR.ID > 0) OR
		                ('{4}' = '%%' AND USR.ID > 0))AND S
                        not exists (SELECT *
                        FROM [dbo].IdentityRoleProfiles AS I join [dbo].IdentityRole as R on I.IdentityRolePvid = R.Pvid
                        where USR.ID = I.IdentityUserPvid AND ((R.Name) = 'SYSTEM ADMIN'))
			
                ) Q
                
                ORDER BY Q.[USRName]", DBString, startIndex, length, AuthConstants.TBL_USER, filter);
            }
        }

        public UserPasswordHistory GetUserPasswordHistory(int UserID)
        {
            UserPasswordHistory user = new UserPasswordHistory();
            var query = getPasswordHistory(UserID.ToString());

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            user.UserID = Convert.ToInt32(dr["UserID"]);
                            user.PasswordHash1 = dr["PasswordHash1"].ToString();
                            user.PasswordDate1 = Convert.ToDateTime(dr["PasswordDate1"]);
                            user.PasswordHash2 = dr["PasswordHash2"].ToString();
                            if (dr["PasswordDate2"].ToString() != "")
                            {
                                user.PasswordDate2 = Convert.ToDateTime(dr["PasswordDate2"]);
                            }
                            user.PasswordHash3 = dr["PasswordHash3"].ToString();
                            if (dr["PasswordDate3"].ToString() != "")
                            {
                                user.PasswordDate3 = Convert.ToDateTime(dr["PasswordDate3"]);
                            }
                            user.PasswordHash4 = dr["PasswordHash4"].ToString();
                            if (dr["PasswordDate4"].ToString() != "")
                            {
                                user.PasswordDate4 = Convert.ToDateTime(dr["PasswordDate4"]);
                            }
                            user.PasswordHash5 = dr["PasswordHash5"].ToString();
                            if (dr["PasswordDate5"].ToString() != "")
                            {
                                user.PasswordDate5 = Convert.ToDateTime(dr["PasswordDate5"]);
                            }
                            user.PasswordHash6 = dr["PasswordHash6"].ToString();
                            if (dr["PasswordDate6"].ToString() != "")
                            {
                                user.PasswordDate6 = Convert.ToDateTime(dr["PasswordDate6"]);
                            }
                            user.PasswordHash7 = dr["PasswordHash7"].ToString();
                            if (dr["PasswordDate7"].ToString() != "")
                            {
                                user.PasswordDate7 = Convert.ToDateTime(dr["PasswordDate7"]);
                            }

                        }
                    }
                }
            }

                        
                return user;
            
        }

        string getPasswordHistory(string userId)
        {
            return string.Format(@"USE {0} SELECT * FROM {1} WHERE UserID = {2}",
               DBString, AuthConstants.TBL_USERPASSWORDHISTORY, userId);
        }

        //UPDATE UserPasswordHistory
        public bool UpdatePasswordHistory(UserPasswordHistory pass)
        {
            string query = updatePassword(pass.UserID.ToString(), pass.PasswordHash1, pass.PasswordDate1.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    int i = command.ExecuteNonQuery();

                    if (i >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                connection.Close();
            }
        }

        string updatePassword(string userId, string hash, string currDate)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                DECLARE 
                @PasswordHash1 varchar(255),
                @PasswordHash2 varchar(255),
                @PasswordHash3 varchar(255),
                @PasswordHash4 varchar(255),
                @PasswordHash5 varchar(255),
                @PasswordHash6 varchar(255),
                @PasswordDate1 datetime,
                @PasswordDate2 datetime,
                @PasswordDate3 datetime,
                @PasswordDate4 datetime,
                @PasswordDate5 datetime,
                @PasswordDate6 datetime

	                -- SET NOCOUNT ON added to prevent extra result sets from
	                -- interfering with SELECT statements.
	                SET NOCOUNT ON;

                    -- Insert statements for procedure here
	                SELECT  @PasswordHash1=PasswordHash1,@PasswordHash2=PasswordHash2,@PasswordHash3=PasswordHash3,
	                @PasswordHash4=PasswordHash4,@PasswordHash5=PasswordHash5,@PasswordHash6=PasswordHash6,
	                @PasswordDate1=PasswordDate1,@PasswordDate2=PasswordDate2,@PasswordDate3=PasswordDate3,
	                @PasswordDate4=PasswordDate4,@PasswordDate5=PasswordDate5,@PasswordDate6=PasswordDate6
	                  FROM dbo.UserPasswordHistory WHERE UserID={1};
                    if @PasswordHash1 <> '{3}'
                    begin
	                if @PasswordHash1 <> ''

	                UPDATE {2}
                   SET [PasswordHash1] = '{3}'
                      ,[PasswordDate1] = '{4}'
                      ,[PasswordHash2] = @PasswordHash1
                      ,[PasswordDate2] = @PasswordDate1
                      ,[PasswordHash3] = @PasswordHash2
                      ,[PasswordDate3] = @PasswordDate2
                      ,[PasswordHash4] = @PasswordHash3
                      ,[PasswordDate4] = @PasswordDate3
                      ,[PasswordHash5] = @PasswordHash4
                      ,[PasswordDate5] = @PasswordDate4
                      ,[PasswordHash6] = @PasswordHash5
                      ,[PasswordDate6] = @PasswordDate5
                      ,[PasswordHash7] = @PasswordHash6
                      ,[PasswordDate7] = @PasswordDate6
                 WHERE UserID={1}

                 Else
                 INSERT INTO {2}
                           ([UserID]
                           ,[PasswordHash1]
                           ,[PasswordDate1])
                     VALUES
                           ({1}
                           ,'{3}'
                           ,'{4}')end", DBString, userId, AuthConstants.TBL_USERPASSWORDHISTORY, hash, currDate);

        }

        public ApplicationUser GetUserByID(int id)
        {
            ApplicationUser user = new ApplicationUser();

            string query = getUserByID(id);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            user.Id = Convert.ToInt32(dr["ID"]);
                            user.UserName = dr["UserName"].ToString();
                            user.Password = dr["Password"].ToString();
                            user.SecurityStamp = dr["SecurityStamp"].ToString();
                            user.Name = dr["Name"].ToString();
                            user.Email = dr["Email"].ToString();
                            user.PasswordHash = dr["Password"].ToString();
                            user.ReceivedEmail = Convert.ToBoolean(dr["ReceivedEmail"]);
                        }
                    }
                }
                connection.Close();
            }
            return user;

        }

        public async Task DeleteUserDetailByIdAsync(int UserId)
        {
            if (UserId >= 0)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == UserId);
                var person = _context.Persons.FirstOrDefault(p => p.UserId == UserId);


                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                
                if (person != null)
                {
                    

                    _context.Persons.Remove(person);
                }
                
                

               
                
            }
            await _context.SaveChangesAsync();
        }

        public List<ApplicationUser> GetAllUsers()
        {
            List<ApplicationUser> user = new List<ApplicationUser>();

            try
            {
                string query = getAllUsers();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    user.Add(new ApplicationUser()
                                    {
                                        Id = Convert.ToInt32(dr["ID"]),
                                        UserName = dr["UserName"].ToString(),
                                        Password = dr["Password"].ToString(),
                                        SecurityStamp = dr["SecurityStamp"].ToString(),
                                        Name = dr["Name"].ToString(),
                                        Email = dr["Email"].ToString(),
                                        Active = Convert.ToBoolean(dr["Active"]),
                                        ReceivedEmail = Convert.ToBoolean(dr["ReceivedEmail"])


                                });

                                }


                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }
            
            return user;
        }
    }

    
}
