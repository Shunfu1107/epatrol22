using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers.Database;
using AdminPortalV7.Libraries.ExtendedUserIdentity.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminPortalV7.Libraries.ExtendedUserIdentity.Services
{
    public class DatabaseService : DatabaseManager
    {
        private SqlConnectionStringBuilder SSB;
        private string DBString;

        public DatabaseService()
        {
            SSB = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            DBString = SSB.InitialCatalog;
        }
        public bool HasTable(string tableName)
        {
            bool isExist = false;
            string query = string.Format(@"  USE {0} SELECT CASE WHEN OBJECT_ID('{1}', 'U') IS NOT NULL THEN 1 ELSE 0 END", DBString, tableName);
            using (var cmd = CreateSqlCommand(query))
            {
                var con = cmd.Connection;
                try
                {
                    con.Open();

                    var reader = default(SqlDataReader);
                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var result = reader.GetInt32(0);
                            isExist = Convert.ToBoolean(result);
                        }
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed) reader.Close();
                    }

                }
                finally
                {
                    if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                }
            }

            return isExist;
        }
        public void CreateDatabaseIfNotExist()
        {
            SqlConnection Conn = new SqlConnection(SSB.ConnectionString); // pass connection string and user must have the permission to create a database,

            string Query = createDatabaseQuery();
            SqlCommand Command = new SqlCommand(Query, Conn);
            try
            {
                Conn.Open();
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        public void CreateUserTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_USER))
            {
                var query = createUserTableQuery(DBString, AuthConstants.TBL_USER);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateModuleTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_MODULE))
            {
                var query = createModuleTableQuery(DBString, AuthConstants.TBL_MODULE);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateModuleOrganizeTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_MODULE_ORGANIZE))
            {
                var query = createModuleOrganizeTableQuery(DBString, AuthConstants.TBL_MODULE_ORGANIZE);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateAcccessLevelTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ACCESS_LEVEL))
            {
                var query = createAccessLevelTableQuery(DBString, AuthConstants.TBL_ACCESS_LEVEL);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateRoleTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ROLE))
            {
                var query = createRoleTableQuery(DBString, AuthConstants.TBL_ROLE);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateRoleAccessTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ROLE_ACCESS))
            {
                var query = createRoleAccessTableQuery(DBString, AuthConstants.TBL_ROLE_ACCESS);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateRoleProfileTableIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ROLEPROFILES))
            {
                var query = createRoleProfileTableQeury(DBString, AuthConstants.TBL_ROLEPROFILES);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateAccessLevelDetailIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ACCESS_LEVEL_DETAIL))
            {
                var query = createAccessLevelDetailTableQuery(DBString, AuthConstants.TBL_ACCESS_LEVEL_DETAIL);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateUserProfileTableQueryIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_USERPROFILES))
            {
                var query = createUserProfileTableQuery(DBString, AuthConstants.TBL_USERPROFILES);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateAccessLevelExclusiveTableQueryIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE))
            {
                var query = createAccessLevelExclusive(DBString, AuthConstants.TBL_ACCESS_LEVEL_EXCLUSIVE);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        public void CreateUserPasswordHistoryTableQueryIfNotExist()
        {
            if (!HasTable(AuthConstants.TBL_USERPASSWORDHISTORY))
            {
                var query = createUserPasswordHistoryTableQuery(DBString, AuthConstants.TBL_USERPASSWORDHISTORY);
                using (var cmd = CreateSqlCommand(query))
                {
                    var con = cmd.Connection;
                    try
                    {
                        con.Open();
                        cmd.ExecuteScalar();
                    }
                    finally
                    {
                        if (con != null && con.State == System.Data.ConnectionState.Open) con.Close();
                    }
                }
            }
        }
        string createDatabaseQuery()
        {
            return string.Format(@"USE [master]
                CREATE DATABASE {0}
                    CONTAINMENT = NONE
                    ON  PRIMARY 
                ( NAME = N'{0}', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQL2K12\MSSQL\DATA\{0}.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
                    LOG ON 
                ( NAME = N'{0}_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQL2K12\MSSQL\DATA\{0}.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
                IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
                begin
                EXEC {0}.[dbo].[sp_fulltext_database] @action = 'enable'
                end
                ALTER DATABASE {0} SET ANSI_NULL_DEFAULT OFF 
                ALTER DATABASE {0} SET ANSI_NULLS OFF 
                ALTER DATABASE {0} SET ANSI_PADDING OFF 
                ALTER DATABASE {0} SET ANSI_WARNINGS OFF 
                ALTER DATABASE {0} SET ARITHABORT OFF 
                ALTER DATABASE {0} SET AUTO_CLOSE OFF 
                ALTER DATABASE {0} SET AUTO_SHRINK OFF 
                ALTER DATABASE {0} SET AUTO_UPDATE_STATISTICS ON 
                ALTER DATABASE {0} SET CURSOR_CLOSE_ON_COMMIT OFF 
                ALTER DATABASE {0} SET CURSOR_DEFAULT  GLOBAL 
                ALTER DATABASE {0} SET CONCAT_NULL_YIELDS_NULL OFF 
                ALTER DATABASE {0} SET NUMERIC_ROUNDABORT OFF 
                ALTER DATABASE {0} SET QUOTED_IDENTIFIER OFF 
                ALTER DATABASE {0} SET RECURSIVE_TRIGGERS OFF 
                ALTER DATABASE {0} SET  DISABLE_BROKER 
                ALTER DATABASE {0} SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                ALTER DATABASE {0} SET DATE_CORRELATION_OPTIMIZATION OFF 
                ALTER DATABASE {0} SET TRUSTWORTHY OFF 
                ALTER DATABASE {0} SET ALLOW_SNAPSHOT_ISOLATION OFF 
                ALTER DATABASE {0} SET PARAMETERIZATION SIMPLE 
                ALTER DATABASE {0} SET READ_COMMITTED_SNAPSHOT OFF 
                ALTER DATABASE {0} SET HONOR_BROKER_PRIORITY OFF 
                ALTER DATABASE {0} SET RECOVERY FULL 
                ALTER DATABASE {0} SET  MULTI_USER 
                ALTER DATABASE {0} SET PAGE_VERIFY CHECKSUM  
                ALTER DATABASE {0} SET DB_CHAINING OFF 
                ALTER DATABASE {0} SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
                ALTER DATABASE {0} SET TARGET_RECOVERY_TIME = 0 SECONDS 
                ALTER DATABASE {0} SET  READ_WRITE", DBString);
        }
        string createUserTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0} 
                    CREATE TABLE {1}(
	                [ID] [int] IDENTITY(1,1) NOT NULL,
	                [UserName] [nvarchar](20) NOT NULL,
	                [Password] [nvarchar](255) NOT NULL,
	                [SecurityStamp] [nvarchar](36) NOT NULL,
	                [Name] [nvarchar](100) NULL,
	                [Email] [nvarchar](100) NULL,
	                [Active] [bit] NULL,
	                [FirstPasswordReset] [bit] NULL,
                 CONSTRAINT [PK_Users_1] PRIMARY KEY CLUSTERED 
                (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ", databaseName, tableName);
        }
        string createModuleTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
                    [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [Name] [nvarchar](50) NOT NULL,
                 CONSTRAINT [PK_IdentityModule] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ", databaseName, tableName);
        }
        string createModuleOrganizeTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	               [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityModulePvid] [bigint] NOT NULL,
	                [PermissionKey] [nvarchar](300) NOT NULL,
                 CONSTRAINT [PK_IdentityModuleOrganize] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ", databaseName, tableName);
        }
        string createAccessLevelTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [Name] [nvarchar](50) NOT NULL,
                 CONSTRAINT [PK_IdentityAccessLevel] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ", databaseName, tableName);
        }
        string createRoleTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [Name] [nvarchar](100) NOT NULL,
	                [StartActiveDate] [datetime] NOT NULL,
	                [EndActiveDate] [datetime] NOT NULL,
	                [IsActive] [bit] NOT NULL,
                 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", databaseName, tableName);
        }
        string createRoleAccessTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityRolePvid] [bigint] NOT NULL,
	                [IdentityModulePvid] [bigint] NOT NULL,
	                [IdentityAccessLevelPvid] [bigint] NOT NULL,
	                [GrantedByIdentityUserPvid] [bigint] NOT NULL,
	                [GrantedDate] [datetime] NOT NULL,
                 CONSTRAINT [PK_IdentityRoleAccess] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", databaseName, tableName);
        }
        string createRoleProfileTableQeury(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityUserPvid] [bigint] NOT NULL,
	                [IdentityRolePvid] [bigint] NOT NULL,
                 CONSTRAINT [PK_GroupProfiles] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", databaseName, tableName);
        }
        string createAccessLevelDetailTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	               [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityAccessLevelPvid] [bigint] NOT NULL,
	                [IdentityModulePvid] [bigint] NULL,
	                [PermissionKey] [nvarchar](300) NOT NULL,
                 CONSTRAINT [PK_IdentityAccessLevelDetail] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", databaseName, tableName);
        }
        string createUserProfileTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityUserPvid] [bigint] NOT NULL,
	                CONSTRAINT [PK_UserProfiles] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ", databaseName, tableName);
        }
        string createAccessLevelExclusive(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	               [Pvid] [bigint] IDENTITY(1,1) NOT NULL,
	                [IdentityUserPvid] [bigint] NOT NULL,
	                [PermissionKey] [nvarchar](400) NOT NULL,
	                [Accessible] [bit] NOT NULL,
                 CONSTRAINT [PK_IdentityUserPermission] PRIMARY KEY CLUSTERED 
                (
	                [Pvid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
                ALTER TABLE {1} ADD  CONSTRAINT [DF_IdentityUserPermission_Accessible]  DEFAULT ((0)) FOR [Accessible]", databaseName, tableName);
        }
        string createUserPasswordHistoryTableQuery(string databaseName, string tableName)
        {
            return string.Format(@"USE {0}
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                CREATE TABLE {1}(
	                [UserID] [int] NOT NULL,
	                [PasswordHash1] [varchar](255) NULL,
	                [PasswordDate1] [datetime] NULL,
	                [PasswordHash2] [varchar](255) NULL,
	                [PasswordDate2] [datetime] NULL,
	                [PasswordHash3] [varchar](255) NULL,
	                [PasswordDate3] [datetime] NULL,
	                [PasswordHash4] [varchar](255) NULL,
	                [PasswordDate4] [datetime] NULL,
	                [PasswordHash5] [varchar](255) NULL,
	                [PasswordDate5] [datetime] NULL,
	                [PasswordHash6] [varchar](255) NULL,
	                [PasswordDate6] [datetime] NULL,
	                [PasswordHash7] [varchar](255) NULL,
	                [PasswordDate7] [datetime] NULL,
                 CONSTRAINT [PK_UserPasswordHistory] PRIMARY KEY CLUSTERED 
                (
	                [UserID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", databaseName, tableName);
        }
    }
}
