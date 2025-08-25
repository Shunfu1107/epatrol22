using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace AdminPortal.Libraries.ExtendedUserIdentity.Attributes
{
    public class ExtendedAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    var info = new JsonInfoResult
                    {
                        Success = false,
                        Code = "401",
                        Exception = new Exception("Login is Required")
                    };

                    context.Result = new JsonResult(info);
                    return;
                }

                context.Result = new ChallengeResult();
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}