using AdminPortalV7.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers.Database;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;




namespace AdminPortalV7.Libraries.ExtendedUserIdentity.Services


{
    public class AccessLevelService : DatabaseManager
    {
        private DataFill dataFill;
        private SqlConnectionStringBuilder SSB;
        private string DBString;

        public AccessLevelService()
        {
            dataFill = new DataFill();
            SSB = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            DBString = SSB.InitialCatalog;
        }
        public AppAccessLevel FindAccessLevel(AccessLevelByNameFilter filter)
        {
            var query = new SQLQueryCommand(accessLevelOnlyQueryCondition("ACC.Name=@Name"), filter);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    var access = default(AppAccessLevel);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dataFill.fillAccessLevel(reader, out access);
                    }

                    return access;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed) reader.Close();
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public AppAccessLevelDetail FindAccessLevelDetail(AccessLevelDetailFilter filter)
        {
            var query = new SQLQueryCommand(accessLevelDetailOnly("ACCDET.IdentityAccessLevelPvid =@AccessLevelPvid AND ACCDET.PermissionKey LIKE @PermissionKey AND ACCDET.IdentityModulePvid=@ModulePvid"), filter);
            using (var cmd = CreateSqlCommand(query))
            {


                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var det = default(AppAccessLevelDetail);
                    while (reader.Read())
                    {
                        dataFill.fillAccessLevelDetail(reader, out det);
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

        public IList<AppAccessLevel> GetAllAccessLevel()
        {
            var query = accessLevelOnlyQueryCondition(string.Empty);
            var con = CreateSqlConnection();
            using (var cmd = new SqlCommand(query, con))
            {


                var reader = default(SqlDataReader);

                try
                {
                    var list = new List<AppAccessLevel>(7);
                    var module = default(AppAccessLevel);
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dataFill.fillAccessLevel(reader, out module);
                        list.Add(module);
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

        public Paging<AppAccessLevel> GetAccessLevels(DataTablePagingFilter filter)
        {
            var query = getAccessLevelList(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.sSearch);
            using (var cmd = CreateSqlCommand(query))
            {


                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var access = default(AppAccessLevel);
                    //var accessdetail = default(AppAccessLevelDetail);
                    var list = new List<AppAccessLevel>(3);
                    //var uniqueAccess = new Dictionary<long, AppAccessLevel>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        //obj = reader["ACCPvid"];
                        //var accpvid = obj == DBNull.Value ? 0 : Convert.ToInt64(obj);
                        //if (!uniqueAccess.Keys.Contains(accpvid))
                        //{
                        dataFill.fillAccessLevel(reader, out access);
                        //    uniqueAccess.Add(accpvid, access);
                        list.Add(access);
                        //}

                        //access = uniqueAccess[accpvid];
                        //fillAccessLevelDetail(reader, out accessdetail);
                        //if (accessdetail != null && !accessdetail.Pvid.Equals(0)) access.AccessDetails.Add(accessdetail);
                    }


                    var dto = new Paging<AppAccessLevel>();
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

        public Paging<AppAccessLevelDetail> GetAccessLevelDetails(DataTablePagingAccessDetailsFilter filter)
        {
            var query = getAccessDetailList(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.sSearch, filter.AccessLevelPvid.ToString());
            using (var cmd = CreateSqlCommand(query))
            {


                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var det = default(AppAccessLevelDetail);
                    var mod = default(AppModule);

                    var list = new List<AppAccessLevelDetail>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        dataFill.fillAccessLevelDetail(reader, out det);
                        dataFill.fillModule(reader, out mod);

                        det.Module = mod;

                        list.Add(det);
                    }


                    var dto = new Paging<AppAccessLevelDetail>();
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

        public Paging<AppAccessLevelExclusive> GetAccessExclusive(DataTablePagingUserRolesFilter filter)
        {
            var query = getExtraRights(filter.iDisplayStart.ToString(), filter.iDisplayLength.ToString(), filter.UserPvid.ToString(), filter.sSearch);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                var reader = default(SqlDataReader);

                try
                {
                    con.Open();
                    reader = cmd.ExecuteReader();
                    var totalRecords = 0;
                    var exclusive = default(AppAccessLevelExclusive);
                    var list = new List<AppAccessLevelExclusive>(3);
                    var obj = default(object);

                    while (reader.Read())
                    {
                        if (totalRecords.Equals(0))
                        {
                            obj = reader["TotalRecords"];
                            totalRecords = obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                        }

                        dataFill.fillAccessLevelExclusive(reader, out exclusive);


                        list.Add(exclusive);
                    }


                    var dto = new Paging<AppAccessLevelExclusive>();
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

        public void AddAccessLevelDetail(AppAccessLevelDetail detail)
        {
            var fields = "IdentityAccessLevelPvid,IdentityModulePvid,PermissionKey";
            var values = "@IdentityAccessLevelPvid,@IdentityModulePvid,@PermissionKey";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ACCESS_LEVEL_DETAIL, fields, values);
            var query = new SQLQueryCommand(cmdText, detail);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;

                try
                {
                    con.Open();
                    detail.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void AddAccessLevel(AppAccessLevel access)
        {
            var fields = "Name";
            var values = "@Name";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ACCESS_LEVEL, fields, values);
            var query = new SQLQueryCommand(cmdText, access);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();
                    access.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public void AddAccessExclusive(AppAccessLevelExclusive access)
        {
            var fields = "IdentityUserPvid,PermissionKey,Accessible";
            var values = "@IdentityUserPvid,@PermissionKey,@Accessible";
            var cmdText = string.Format("INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY", AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE, fields, values);
            var query = new SQLQueryCommand(cmdText, access);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;

                try
                {
                    con.Open();
                    access.Pvid = Convert.ToInt64(cmd.ExecuteScalar());
                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }
        }
        public bool UpdateAccessLevel(UpdateAccessLevelFilter filter)
        {
            var values = "Name=@Name";
            var cmdText = string.Format("UPDATE {0} SET {1} WHERE Pvid=@AccessLevelPvid", AuthConstants.TBL_ACCESS_LEVEL, values);
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
        public bool UpdateAccessLevelExclusive(UpdateAccessLevelExclusiveFilter filter)
        {
            var values = "Accessible=@Accessible,PermissionKey=@PermissionKey"; ;
            var cmdText = string.Format("UPDATE {0} SET {1} WHERE Pvid=@ExclusivePvid", AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE, values);
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
        public bool UpdateAccessLevelDetail(UpdateAccessLevelDetailFilter filter)
        {
            var values = "IdentityModulePvid=@BelongToModulePvid";
            var cmdText = string.Format("UPDATE {0} SET {1} WHERE PermissionKey=@PermissionKey", AuthConstants.TBL_ACCESS_LEVEL_DETAIL, values);
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
        public void DeleteAccessLevelDetails(DeleteAccessLevelDetailFilter filter)
        {

            var cmdText = string.Format("DELETE FROM {0} WHERE PermissionKey LIKE @PermissionKey AND IdentityModulePvid=@IdentityModulePvid AND IdentityAccessLevelPvid=@IdentityAccessLevelPvid ", AuthConstants.TBL_ACCESS_LEVEL_DETAIL);
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
        public void DeleteAccessLevel(DeleteAccessLevelFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE Pvid=@AccessLevelPvid", AuthConstants.TBL_ACCESS_LEVEL);
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
        public void DeleteAccessLevelExclusive(DeleteAccessExclusiveFilter filter)
        {
            var cmdText = string.Format("DELETE FROM {0} WHERE Pvid=@ExclusivePvid", AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE);
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
        string accessLevelOnlyQueryCondition(string condition)
        {
            return string.Format(@"SELECT 
                ACC.Pvid ACCPvid, ACC.Name ACCName
		        FROM {0} ACC WITH(NOLOCK)
                {1}",
                AuthConstants.TBL_ACCESS_LEVEL,
                string.IsNullOrEmpty(condition) ? string.Empty : " WHERE " + condition);
        }
        string accessLevelDetailOnly(string condition)
        {
            return string.Format(@"SELECT 
                ACCDET.Pvid ACCDETPvid, ACCDET.IdentityAccessLevelPvid ACCDETIdentityAccessLevelPvid,ACCDET.PermissionKey ACCDETPermissionKey,ACCDET.IdentityModulePvid ACCDETIdentityModulePvid
		        FROM {0} ACCDET WITH(NOLOCK)
                {1}",
                AuthConstants.TBL_ACCESS_LEVEL_DETAIL,
                string.IsNullOrEmpty(condition) ? string.Empty : " WHERE " + condition);
        }

        string getAccessLevelList(string startIndex, string length, string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = "%" + filter + "%";
            }
            return string.Format(@"USE {0}
                SET NOCOUNT ON;DECLARE @endpos int
                DECLARE @StartIndex int
                SET @StartIndex = {1} +1
                SET @endpos = @StartIndex + {2} -1

                SELECT * FROM 
                (
	                -- Total records, a bit redundant but only need one time select
	                SELECT COUNT(1) OVER() AS TotalRecords,
	                -- Row number
	                ROW_NUMBER() OVER(ORDER BY ACC.Name ASC) AS RowNumber
		
	                ,ACC.Pvid ACCPvid
	                ,ACC.Name ACCName

	                --,ACCDET.IdentityAccessLevelPvid ACCDETIdentityAccessLevelPvid
	                --,ACCDET.IdentityModulePvid ACCDETIdentityModulePvid
	                --,ACCDET.PermissionKey ACCDETPermissionKey
	                --,ACCDET.Pvid ACCDETPvid	

	                FROM {3} ACC WITH(NOLOCK)
	                --LEFT JOIN IdentityAccessLevelDetail ACCDET ON ACCDET.IdentityAccessLevelPvid = ACC.Pvid
	                WHERE 
	                --(@Filter<> '' AND (ACC.Name LIKE @Filter OR ACCDET.PermissionKey LIKE @Filter)) OR
	                ('{4}' <> '' AND (ACC.Name LIKE '{4}')) OR
		                ('{4}' = '' AND ACC.Pvid > 0) OR
		                ('{4}' = '%' AND ACC.Pvid > 0) OR
		                ('{4}' = '%%' AND ACC.Pvid > 0)
                ) Q
                WHERE 
                ({2} = -1 AND Q.RowNumber >= 0) OR
                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, startIndex, length, AuthConstants.TBL_ACCESS_LEVEL, filter);
        }

        string getAccessDetailList(string startIndex, string length, string filter, string accessId)
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
	                ROW_NUMBER() OVER(ORDER BY ACCDET.PermissionKey  ASC) AS RowNumber
		
	                ,ACCDET.Pvid ACCDETPvid
	                ,ACCDET.IdentityAccessLevelPvid ACCDETIdentityAccessLevelPvid
	                ,ACCDET.IdentityModulePvid ACCDETIdentityModulePvid
	                ,ACCDET.PermissionKey ACCDETPermissionKey

	                ,[MOD].Pvid MODPvid
	                ,[MOD].Name MODName
		
	                FROM {3} ACCDET
	                LEFT JOIN IdentityModule [MOD] ON [MOD].Pvid = ACCDET.IdentityModulePvid
	                WHERE 
	                ACCDET.IdentityAccessLevelPvid = {4} 
	                AND(
		                ('{5}' <> '' AND (ACCDET.PermissionKey LIKE '{5}' OR [MOD].Name LIKE '{5}')) 
	                OR ('{5}' = '' AND ACCDET.Pvid > 0)
	                OR ('{5}' = '%' AND ACCDET.Pvid > 0)
	                OR ('{5}' = '%%' AND ACCDET.Pvid > 0)
	                )
                ) Q
                WHERE 
                ({2} = -1 AND Q.RowNumber >= 0) OR
                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, startIndex, length, AuthConstants.TBL_ACCESS_LEVEL_DETAIL, accessId, filter);
        }

        string getExtraRights(string start, string length, string user, string filter)
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
		                ROW_NUMBER() OVER(ORDER BY ACCEX.Pvid  ASC) AS RowNumber
		
		                , ACCEX.Pvid ACCEXPvid
		                , ACCEX.IdentityUserPvid ACCEXIdentityUserPvid
		                , ACCEX.PermissionKey ACCEXPermissionKey
		                , ACCEX.Accessible ACCEXAccessible

		                FROM {3} ACCEX
		                JOIN [User] USR ON USR.ID = ACCEX.IdentityUserPvid
		                WHERE 
		                ACCEX.IdentityUserPvid = {4}
		                AND(
		                 ('{5}' <> '' AND (USR.Name LIKE '{5}' OR USR.UserName LIKE '{5}' OR USR.Email LIKE '{5}')) 
		                OR ('{5}' = '' AND ACCEX.Pvid > 0)
		                OR ('{5}' = '%' AND ACCEX.Pvid > 0)
		                OR ('{5}' = '%%' AND ACCEX.Pvid > 0)
		                )
                    ) Q
                    WHERE 
	                ({2} = -1 AND Q.RowNumber >= 0) OR
	                (Q.RowNumber BETWEEN @StartIndex AND @endpos)", DBString, start, length, AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE, user, filter);
        }
    }
}
