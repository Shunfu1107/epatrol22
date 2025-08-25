using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database
{
    public class SQLQueryCommand : SQLBaseCommand
    {
        public SQLQueryCommand(object filter) : this(string.Empty, filter) { }
        public SQLQueryCommand(string cmdText, object filter)
            : base(cmdText, filter)
        {
            base.ParameterPrefix = "@";
        }

        public override System.Data.Common.DbParameter[] GetParameters()
        {
            return base.GetParameters();
        }

        protected override void SetPercentSuffix(ref object value, StringPercentMark suffix)
        {
            switch (suffix)
            {
                case StringPercentMark.Left:
                    value = ("%" + value);
                    return;
                case StringPercentMark.Right:
                    value = (value + "%");
                    return;
                case StringPercentMark.Both:
                    value = ("%" + value + "%");
                    return;
            }
        }

        protected override DbParameter[] GenerateParameter(IList<ParameterizeQueryAttribute> attributes)
        {
            var sqlParams = new SqlParameter[attributes.Count];
            for (int i = 0; i < attributes.Count; i++)
            {
                var item = attributes[i];
                var valueParsing = IsDefaultValue(item.Value) && !IsDefaultValue(item.DefaultValue) ? item.DefaultValue : item.Value;
                var value = convertToDBValue(item.DbType, valueParsing, item.Format);
                var dbType = item.DbType;// (string.IsNullOrWhiteSpace(item.Format)) ? item.DbType : SqlDbType.NVarChar;
                if (value != null) SetPercentSuffix(ref value, item.StringPercentMark);
                sqlParams[i] = new SqlParameter(this.ParameterPrefix + item.Name, dbType);
                sqlParams[i].Value = value == null ? DBNull.Value : value;
                //item.ParameterValue = value;
                item.ParameterValue = sqlParams[i].Value;
            }

            return sqlParams;
        }

        private object convertToDBValue(System.Data.SqlDbType type, object value, string format)
        {
            switch (type)
            {
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    var datetime = (DateTime)value;
                    if (string.IsNullOrWhiteSpace(format)) return datetime;
                    return datetime.ToString(format);
                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.NChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    var temp = Convert.ToString(value);
                    return string.IsNullOrWhiteSpace(temp) ? /*DBNull.Value*/ string.Empty : value;

                default:
                    return value;
            }
        }
    }
}