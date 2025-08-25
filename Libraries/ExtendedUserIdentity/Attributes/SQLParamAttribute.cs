using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SQLParamAttribute : Attribute
    {
        public DbType DataType;
        public ParameterDirection ParamDirection;
        public SQLParamAttribute()
        {

        }
    }
}
