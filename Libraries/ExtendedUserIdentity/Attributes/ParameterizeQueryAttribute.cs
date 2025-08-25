using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using System.Data;
using System.Diagnostics;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [DebuggerDisplay("{Name} : {ParameterValue}")]
    public sealed class ParameterizeQueryAttribute : Attribute
    {
        public ParameterizeQueryAttribute()
        {
            Orders = 100;
            IsRequired = true;
        }


        public string Name { get; set; }

        public SqlDbType DbType { get; set; }

        public StringPercentMark StringPercentMark { get; set; }

        public int Orders { get; set; }

        public string Format { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or Set constant value of the parameter
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// <para>Gets or Set default value of the underlying value if 0/null/empty based on their type</para>
        /// <para>and will combain with StringPercentMark</para>
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Debug purpose only !!!
        /// </summary>
        internal object ParameterValue { get; set; }
    }
}
