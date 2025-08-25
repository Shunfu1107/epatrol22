using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Filters
{
    public class DataTablePagingFilter : BaseFilter
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        [ParameterizeQuery(DbType = SqlDbType.NVarChar, Name = "Filter", StringPercentMark = StringPercentMark.Both)]
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        [ParameterizeQuery(DbType = SqlDbType.Int, Name = "Length")]
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        [ParameterizeQuery(DbType = SqlDbType.Int, Name = "StartIndex")]
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public string robot_serialnum { get; set; }
    }
}
