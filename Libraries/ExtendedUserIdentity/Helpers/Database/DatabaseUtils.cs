using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.Database;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database
{
    public class DatabaseUtils
    {
        public static List<SQLParamEntity> GetNewObjectLog(object log, PropertyInfo[] propInfos)
        {
            var list = new List<SQLParamEntity>();

            for (int i = 0; i < propInfos.Length; i++)
            {
                var info = propInfos[i];
                var customattrs = info.GetCustomAttributes(false);
                for (int x = 0; x < customattrs.Length; x++)
                {
                    var customatt = customattrs[x];
                    if (customatt.GetType() == typeof(SQLParamAttribute))
                    {

                        var propName = info.Name;
                        var propValue = info.GetValue(log, null);

                        SQLParamAttribute c = (SQLParamAttribute)customatt;

                        if (propValue != null)
                            list.Add(new SQLParamEntity { Value = propValue.ToString(), ReferenceColumnName = propName, DataType = c.DataType, ParamDirection = c.ParamDirection });
                    }
                }
            }
            return list;
        }


        internal static SqlParameter[] GetSQLParameter(object parameter)
        {
            List<SQLParamEntity> newList = new List<SQLParamEntity>();

            var paramType = parameter.GetType();
            var paramPropType = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            newList = GetNewObjectLog(parameter, paramPropType);


            SqlParameter[] sqlparam = new SqlParameter[newList.Count];
            for (int i = 0; i <= newList.Count - 1; i++)
            {
                sqlparam[i] = new SqlParameter();
                sqlparam[i].ParameterName = "@" + newList[i].ReferenceColumnName;
                sqlparam[i].Value = newList[i].Value;
                sqlparam[i].DbType = newList[i].DataType;
                sqlparam[i].Direction = newList[i].ParamDirection;
            }

            return sqlparam;
        }

        public void CreateReader(Action<SqlConnection> callback)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(DatabaseConfiguration.ConnectionString);
                conn.Open();
                callback.Invoke(conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }

        internal static void CreateTransaction(Action<SqlConnection, SqlTransaction> callback)
        {
            SqlConnection scon = null;
            SqlTransaction sqlTrans = null;
            try
            {
                scon = new SqlConnection(DatabaseConfiguration.ConnectionString);
                scon.Open();
                sqlTrans = scon.BeginTransaction();

                callback.Invoke(scon, sqlTrans);
                sqlTrans.Commit();
            }
            catch (Exception)
            {
                sqlTrans.Rollback();
                throw;
            }
            finally
            {
                if (scon != null)
                {
                    if (scon.State == ConnectionState.Open)
                        scon.Close();
                }
            }
        }



        public static object RunExecuteScalar(SqlConnection scon, string Command, bool isStoreProcedure, SqlParameter[] sqlParam = null, int timeout = 0)
        {
            object returnValue = null;
            try
            {


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = scon;

                if (timeout != 0)
                    cmd.CommandTimeout = timeout;

                if (isStoreProcedure == false)
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = Command;

                if (sqlParam != null)
                {
                    for (int j = 0; j < sqlParam.Length; j++)
                        cmd.Parameters.Add(sqlParam[j]);

                }
                returnValue = cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }

            return returnValue;
        }


        public static SqlDataReader RunCustomExecuteNonQuery(SqlConnection scon, SqlTransaction transaction, String Command, bool isStoreProcedure, SqlParameter[] sqlParam = null, int timeout = 0)
        {
            SqlDataReader reader = null;

            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = scon;
                cmd.Transaction = transaction;

                if (timeout != 0)
                    cmd.CommandTimeout = timeout;

                if (isStoreProcedure == false)
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = Command;

                if (sqlParam != null)
                {
                    for (int j = 0; j < sqlParam.Length; j++)
                        cmd.Parameters.Add(sqlParam[j]);
                }

                reader = cmd.ExecuteReader();
            }
            catch
            {
                throw;
            }
            return reader;
        }

        public static SqlDataReader RunExecuteReader(SqlConnection scon, string Command, bool isStoreProcedure, SqlParameter[] sqlParam = null, int timeout = 0)
        {
            SqlDataReader reader = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = scon;

                if (timeout != 0)
                    cmd.CommandTimeout = timeout;

                if (isStoreProcedure == false)
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = Command;

                if (sqlParam != null)
                {
                    for (int j = 0; j < sqlParam.Length; j++)
                        cmd.Parameters.Add(sqlParam[j]);


                }

                reader = cmd.ExecuteReader();
            }
            catch
            {
                throw;
            }
            return reader;
        }

        public static SqlCommand RunExecuteNonQuery(SqlConnection scon, SqlTransaction transaction, String Command, bool isStoreProcedure, SqlParameter[] sqlParam = null, int timeout = 0)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = scon;
                cmd.Transaction = transaction;

                if (timeout != 0)
                    cmd.CommandTimeout = timeout;

                if (isStoreProcedure == false)
                    cmd.CommandType = CommandType.Text;
                else
                    cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandText = Command;

                if (sqlParam != null)
                {
                    for (int j = 0; j < sqlParam.Length; j++)
                        cmd.Parameters.Add(sqlParam[j]);
                }

                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return cmd;
        }

    }
}