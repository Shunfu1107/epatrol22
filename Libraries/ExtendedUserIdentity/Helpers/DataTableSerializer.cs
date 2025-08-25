using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.AspNetCore.Mvc;


namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public class DataTableSerializer
    {
        public static JsonResult SerializeDataTable<T>(string echo, Paging<T> dto)
        {

            //var result = new JsonResult();
            
            var data = (new
            {
                Data = dto.List,
                sEcho = echo,
                iTotalRecords = dto.PagingInfo.TotalRecords,
                //iTotalDisplayRecords = dto.PagingInfo.RecordPerPage
                iTotalDisplayRecords = dto.PagingInfo.TotalRecords
            });
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            
            return new JsonResult(data);
        }

        internal static void SerializeDataTable(AppUser result)
        {
            throw new NotImplementedException();
        }
    }
}