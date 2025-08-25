using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.Database
{
    public class SQLParamEntity
    {
        public string ReferenceColumnName { get; set; }
        public string Value { get; set; }
        public DbType DataType { get; set; }
        public ParameterDirection ParamDirection { get; set; }
    }
}