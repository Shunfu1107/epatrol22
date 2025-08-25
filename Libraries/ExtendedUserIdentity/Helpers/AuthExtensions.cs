using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public static class AuthExtensions
    {
        public static IList<IContent> Clone(this IList<IContent> contents)
        {
            IList<IContent> content = new List<IContent>(3);
            for (var i = 0; i < contents.Count; i++)
            {
                content.Add(contents[i]);
            }
            return content;
        }

        public static bool HasColumn(this IDataRecord record, string columnName)
        {
            try
            {
                return record.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}