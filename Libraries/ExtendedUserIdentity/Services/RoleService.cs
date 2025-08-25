using AdminPortalV7.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers.Database;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Models;
using Microsoft.Data.SqlClient;

namespace AdminPortalV7.Libraries.ExtendedUserIdentity.Services
{
    public class RoleService : DatabaseManager
    {
        private DataFill dataFill;
        private SqlConnectionStringBuilder SSB;
        private string DBString;
        private readonly IConfiguration _configurtion;

        

        public RoleService(IConfiguration configuration)
        {            
            _configurtion = configuration;

            dataFill = new DataFill();
            SSB = new SqlConnectionStringBuilder(_configurtion.GetConnectionString("DefaultConnection"));
            DBString = SSB.InitialCatalog;
        }
        public AppRole FindRole(RoleByNameFilter filter)
        {
            var query = new SQLQueryCommand(roleOnly("ROL.Name LIKE @Name"), filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var det = default(AppRole);
                    while (reader.Read())
                    {
                        dataFill.fillRole(reader, out det);
                    }

                    return det;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public AppRoleAccess FindRoleAccess(RoleAccessFilter filter)
        {
            var query = new SQLQueryCommand(roleAccessOnly("ROLAC.IdentityRolePvid=@RolePvid AND ROLAC.IdentityModulePvid=@ModulePvid AND ROLAC.IdentityAccessLevelPvid=@AccessLevelPvid"), filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var det = default(AppRoleAccess);
                    while (reader.Read())
                    {
                        dataFill.fillRoleAccess(reader, out det);
                    }

                    return det;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public IList<AppRoleProfiles> FindRoleProfiles(RoleProfilesByUser filter)
        {
            var query = new SQLQueryCommand(roleProfilesOnly("ROLPRO.IdentityUserPvid=@UserPvid"), filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var rolpro = default(AppRoleProfiles);
                    var list = new List<AppRoleProfiles>(3);

                    while (reader.Read())
                    {
                        dataFill.fillRoleProfile(reader, out rolpro);
                        list.Add(rolpro);
                    }
                    return list;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public IList<AppRole> GetRoles()
        {
            var query = roleOnly(string.Empty);
            using (var cmd = CreateSqlCommand(query))
            {

                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    var list = new List<AppRole>(7);
                    var role = default(AppRole);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dataFill.fillRole(reader, out role);
                        if (role != null && !role.Pvid.Equals(0)) list.Add(role);
                    }

                    return list;

                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public Paging<AppRole> GetRoles(DataTablePagingFilter filter)
        {
            var query = getRole(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.sSearch);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var role = default(AppRole);
                    var list = new List<AppRole>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        dataFill.fillRole(reader, out role);
                        list.Add(role);
                    }


                    var dto = new Paging<AppRole>();
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = totalRecords
                    };
                    dto.PagingInfo = info;
                    dto.List = list;

                    return dto;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public Paging<AppRoleAccess> GetRoleAccessDetails(DataTablePagingRoleAccessFilter filter)
        {
            var query = getRoleAccessList(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.sSearch, filter.RolePvid.ToString());
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var roleacc = default(AppRoleAccess);
                    var acc = default(AppAccessLevel);
                    var mod = default(AppModule);
                    var usr = default(AppUser);

                    var list = new List<AppRoleAccess>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        dataFill.fillRoleAccess(reader, out roleacc);
                        dataFill.fillAccessLevel(reader, out acc);
                        dataFill.fillModule(reader, out mod);
                        dataFill.fillUser(reader, out usr);

                        roleacc.AccessLevel = acc;
                        roleacc.Module = mod;
                        roleacc.GrantedBy = usr;

                        list.Add(roleacc);
                    }


                    var dto = new Paging<AppRoleAccess>();
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = totalRecords
                    };
                    dto.PagingInfo = info;
                    dto.List = list;

                    return dto;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public Paging<AppRole> GetUserRoles(DataTablePagingUserRolesFilter filter)
        {
            var query = getRole(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.UserPvid.ToString(), filter.sSearch);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var role = default(AppRole);
                    var list = new List<AppRole>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        dataFill.fillRole(reader, out role);
                        list.Add(role);
                    }


                    var dto = new Paging<AppRole>();
                    var info = new PagingInfo
                    {
                        CurrentPage = (filter.iDisplayStart / filter.iDisplayLength) + 1,
                        RecordPerPage = filter.iDisplayLength,
                        TotalRecords = totalRecords
                    };
                    dto.PagingInfo = info;
                    dto.List = list;

                    return dto;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public void AddRole(AppRole role)
        {
            var fields = "Name,StartActiveDate,EndActiveDate,IsActive";
            var values = "@Name,@StartActiveDate,@EndActiveDate,@IsActive";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ROLE, fields, values);
            var query = new SQLQueryCommand(cmdText, role);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    role.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void AddRoleAccess(AppRoleAccess accessrights)
        {
            var fields = "IdentityRolePvid,IdentityModulePvid,IdentityAccessLevelPvid,GrantedByIdentityUserPvid,GrantedDate";
            var values = "@IdentityRolePvid,@IdentityModulePvid,@IdentityAccessLevelPvid,@GrantedByIdentityUserPvid,@GrantedDate";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ROLE_ACCESS, fields, values);
            var query = new SQLQueryCommand(cmdText, accessrights);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    accessrights.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void AddRoleProfiles(AppRoleProfiles profile)
        {
            var fields = "IdentityUserPvid,IdentityRolePvid";
            var values = "@IdentityUserPvid,@IdentityRolePvid";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ROLEPROFILES, fields, values);
            var query = new SQLQueryCommand(cmdText, profile);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    profile.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }

        public bool UpdateRole(UpdateRoleFilter filter)
        {
            var values = "Name=@Name,StartActiveDate=@StartActiveDate,EndActiveDate=@EndActiveDate,IsActive=@IsActive"; ;
            var cmdText = string.Format("UPDATE {0} SET {1} WHERE Pvid=@RolePvid", AuthConstants.TBL_ROLE, values);
            var query = new SQLQueryCommand(cmdText, filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() != 0;
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void DeleteRoleProfile(DeleteRoleProfileFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE IdentityUserPvid=@UserPvid AND IdentityRolePvid=@RolePvid", AuthConstants.TBL_ROLEPROFILES);
            var query = new SQLQueryCommand(cmdText, filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void DeleteAllRoleProfile(DeleteRoleProfileFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE IdentityUserPvid=@UserPvid", AuthConstants.TBL_ROLEPROFILES);
            var query = new SQLQueryCommand(cmdText, filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void DeleteRole(DeleteRoleFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE Pvid=@RolePvid", AuthConstants.TBL_ROLE);
            var query = new SQLQueryCommand(cmdText, filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void DeleteRoleAccess(DeleteRoleAccessFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE Pvid=@RoleAccessPvid", AuthConstants.TBL_ROLE_ACCESS);
            var query = new SQLQueryCommand(cmdText, filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        string roleOnly(string condition)
        {
            return string.Format(@"SELECT 
                ROL.Pvid ROLPvid, ROL.Name ROLName,ROL.StartActiveDate ROLStartActiveDate,ROL.EndActiveDate ROLEndActiveDate, ROL.IsActive ROLIsActive
		        FROM {0} ROL WITH(NOLOCK)
                {1}",
                AuthConstants.TBL_ROLE,
                string.IsNullOrEmpty(condition) ? string.Empty : " WHERE " + condition);
        }
        string roleAccessOnly(string condition)
        {
            return string.Format(@"SELECT 
                ROLAC.Pvid ROLACPvid, ROLAC.IdentityRolePvid ROLACIdentityRolePvid,ROLAC.IdentityModulePvid ROLACIdentityModulePvid,ROLAC.IdentityAccessLevelPvid ROLACIdentityAccessLevelPvid, ROLAC.GrantedByIdentityUserPvid ROLACGrantedByIdentityUserPvid,ROLAC.GrantedDate ROLACGrantedDate
		        FROM {0} ROLAC WITH(NOLOCK)
                {1}",
                AuthConstants.TBL_ROLE_ACCESS,
                string.IsNullOrEmpty(condition) ? string.Empty : " WHERE " + condition);
        }
        string roleProfilesOnly(string condition)
        {
            return string.Format(@"SELECT 
                ROLPRO.Pvid ROLPROPvid, ROLPRO.IdentityUserPvid ROLPROIdentityUserPvid,ROLPRO.IdentityRolePvid ROLPROIdentityRolePvid
		        FROM {0} ROLPRO WITH(NOLOCK)
                {1}",
                AuthConstants.TBL_ROLEPROFILES,
                string.IsNullOrEmpty(condition) ? string.Empty : " WHERE " + condition);
        }

        string getRole(string startIndex, string length, string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = "%" + filter + "%";
            }
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;DECLARE @endpos int
                DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SET @endpos = @StartIndex + {2} -1

                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY ROL.Name ASC) AS RowNumber
		
	                ,ROL.Pvid ROLPvid
	                ,ROL.Name ROLName
	                ,ROL.StartActiveDate ROLStartActiveDate
	                ,ROL.EndActiveDate ROLEndActiveDate
	                ,ROL.IsActive ROLIsActive

	                FROM {3} ROL WITH(NOLOCK)
	                WHERE 
	                ('{4}' <> '' AND (ROL.Name LIKE '{4}')) OR
		                ('{4}' = '' AND ROL.Pvid > 0) OR
		                ('{4}' = '%' AND ROL.Pvid > 0) OR
		                ('{4}' = '%%' AND ROL.Pvid > 0)
                ) Q
                WHERE 
                ({2} = -1 AND Q.RowNumber >= 0) OR
                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, startIndex, length, AuthConstants.TBL_ROLE, filter);
        }

        string getRoleAccessList(string startIndex, string length, string filter, string roleId)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = "%" + filter + "%";
            }
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET NOCOUNT ON;DECLARE @endpos int
                DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SET @endpos = @StartIndex + {2} -1

                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY [MOD].Name ASC) AS RowNumber
		
	                ,ROLAC.Pvid ROLACPvid
	                ,ROLAC.GrantedByIdentityUserPvid ROLACGrantedByIdentityUserPvid
	                ,ROLAC.GrantedDate ROLACGrantedDate
	                ,ROLAC.IdentityAccessLevelPvid ROLACIdentityAccessLevelPvid
	                ,ROLAC.IdentityModulePvid ROLACIdentityModulePvid
	                ,ROlAC.IdentityRolePvid ROlACIdentityRolePvid

	                ,ACC.Pvid ACCPvid
	                ,ACC.Name ACCName

	                ,[MOD].Pvid MODPvid
	                ,[MOD].Name MODName

	                ,USR.ID USRPvid
	                ,USR.Email USREmail
	                ,USR.Name USRName
	                ,USR.Password USRPasswordHash
	                ,USR.SecurityStamp USRSecurityStamp
	                ,USR.UserName USRUserName
	                ,USR.Active USRActive
                    ,PER.NRIC
					,PER.Mobile

	                FROM {3} ROLAC WITH(NOLOCK)
	                LEFT JOIN IdentityAccessLevel ACC ON ACC.Pvid = ROLAC.IdentityAccessLevelPvid
	                LEFT JOIN IdentityModule [MOD] ON [MOD].Pvid = ROLAC.IdentityModulePvid
	                LEFT JOIN [User] USR ON USR.ID = ROLAC.GrantedByIdentityUserPvid
                    LEFT JOIN Persons PER ON USR.ID = PER.UserId

	                WHERE 
	                ROLAC.IdentityRolePvid = {4} AND 
	                (
		                ('{5}' <> '' AND (ACC.Name LIKE '{5}' OR [MOD].Name LIKE '{5}' OR USR.Name LIKE '{5}' AND ACC.Pvid > 0)) OR
		                ('{5}' = '' AND ACC.Pvid > 0) OR
		                ('{5}' = '%' AND ACC.Pvid > 0) OR
		                ('{5}' = '%%' AND ACC.Pvid > 0)
	                )
                ) Q
                WHERE
                ({2} = -1 AND Q.RowNumber >= 0) OR
                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, startIndex, length, AuthConstants.TBL_ROLE_ACCESS, roleId, filter);
        }

        string getRole(string start, string length, string user, string filter)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                -- SET NOCOUNT ON added to prevent extra result sets from
                -- interfering with SELECT statements.
                SET NOCOUNT ON;
                DECLARE @endpos int
                DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SET @endpos = @StartIndex + {2} -1
	

                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY ROL.Name  ASC) AS RowNumber
		
	                , ROL.Pvid ROLPvid
	                , ROL.Name ROLName
	                , ROL.StartActiveDate ROLStartActiveDate
	                , ROL.EndActiveDate ROLEndActiveDate
	                , ROL.IsActive ROLIsActive
	
	                FROM {3} ROLPRO
	                JOIN IdentityRole ROL ON ROL.Pvid = ROLPRO.IdentityRolePvid
	                WHERE 
	                ROLPRO.IdentityUserPvid = {4}
	                AND(('{5}'<> '' AND (ROL.Name LIKE '{5}')) 
	                OR ('{5}' = '' AND ROL.Pvid > 0)
	                OR ('{5}' = '%' AND ROL.Pvid > 0)
	                OR ('{5}' = '%%' AND ROL.Pvid > 0)
	                )
                ) Q
                WHERE 
                ({2} = -1 AND Q.RowNumber >= 0) OR
                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, start, length, AuthConstants.TBL_ROLEPROFILES, user, filter);
        }
    }
}
