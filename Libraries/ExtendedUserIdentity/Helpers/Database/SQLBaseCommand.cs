using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database
{
    public abstract class SQLBaseCommand
    {
        private object m_filter;
        private string m_query;


        protected DbParameter[] Parameters { get; set; }

        protected ParameterizeQueryAttribute[] Attributes { get; private set; }

        protected string ParameterPrefix { get; set; }

        public CommandType CommandType
        {
            get
            {
                if (string.IsNullOrEmpty(m_query)) return CommandType.Text;

                var queryTemp = m_query.ToLower();

                /*
                return !queryTemp.StartsWith("select") &&
                    !queryTemp.StartsWith("update") &&
                    !queryTemp.StartsWith("delete") &&
                    !queryTemp.StartsWith("insert into") ? CommandType.StoredProcedure : CommandType.Text;
                */

                return (queryTemp.StartsWith("select") && queryTemp.Contains("from")) ||
                       (queryTemp.StartsWith("update") && queryTemp.Contains("set")) ||
                       (queryTemp.StartsWith("delete") && queryTemp.Contains("where")) ||
                       queryTemp.StartsWith("insert into") ? CommandType.Text : CommandType.StoredProcedure;
            }
        }




        public SQLBaseCommand(object filter) : this(string.Empty, filter) { }

        public SQLBaseCommand(string cmdText, object filter)
        {
            this.m_filter = filter;
            this.m_query = cmdText;
        }




        public virtual DbParameter[] GetParameters()
        {
            var paramList = GetAttributes(this.m_filter);
            var orderedList = paramList.OrderBy(m => m.Orders);

            this.Parameters = this.GenerateParameter(orderedList.ToList());
            this.Attributes = paramList.ToArray();
            return this.Parameters;
        }

        protected IList<ParameterizeQueryAttribute> GetAttributes(object filter, Action<ParameterizeQueryAttribute> callback = null)
        {
            PropertyInfo[] proInfos = filter.GetType().GetProperties();
            IList<ParameterizeQueryAttribute> listParameter = new List<ParameterizeQueryAttribute>(3);

            for (int i = 0; i < proInfos.Length; i++)
            {
                var property = proInfos[i];
                var attrs = property.GetCustomAttributes(typeof(ParameterizeQueryAttribute), true);
                if (attrs == null || attrs.Length == 0) continue;

                var attribute = (ParameterizeQueryAttribute)attrs[0];
                if (attribute.Value == null) attribute.Value = property.GetValue(filter, BindingFlags.Instance | BindingFlags.GetProperty, null, null, null);
                if (!attribute.IsRequired && IsDefaultValue(attribute.Value)) continue;
                if (string.IsNullOrWhiteSpace(attribute.Name)) attribute.Name = property.Name;

                listParameter.Add(attribute);

                if (callback != null) callback.Invoke(attribute);
            }

            return listParameter;
        }

        public static implicit operator string(SQLBaseCommand obj)
        {
            return obj.m_query;
        }

        protected bool IsDefaultValue(object value)
        {
            if (value == null) return true;
            var type = value.GetType();
            if (type == typeof(string)) return string.IsNullOrWhiteSpace((string)value);
            else if (type.IsValueType && type == typeof(Guid)) return value.ToString() == Guid.Empty.ToString();
            else if (type.IsValueType && type != typeof(DateTime)) return 0 == Convert.ToDouble(value);
            else if (type == typeof(DateTime))
                return DateTime.MinValue == Convert.ToDateTime(value) ||
                       DateTime.MaxValue == Convert.ToDateTime(value) ||
                       SqlDateTime.MinValue.Value == Convert.ToDateTime(value) ||
                       SqlDateTime.MaxValue.Value == Convert.ToDateTime(value);
            return false;
        }

        protected abstract DbParameter[] GenerateParameter(IList<ParameterizeQueryAttribute> attributes);

        protected abstract void SetPercentSuffix(ref object value, StringPercentMark suffix);
    }
}