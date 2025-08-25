using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes;
using Microsoft.AspNetCore.Identity;
using AdminPortalV8.Data.ExtendedIdentity;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ContentPermissionAttribute : ContentGenericAttribute, IContent
    {
        
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ContentPermissionAttribute()
            : base()
        {
            Order = 1;
            _contextAccessor = new HttpContextAccessor();
            
        }

        public ContentPermissionAttribute(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _contextAccessor = httpContextAccessor;
        }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var result = new JsonInfoResult();
            var authorized = true;
            //var user = ((ClaimsIdentity)WebAuthentication.AuthenticationManager.User.Identity); //14/10/23
            var user = (_contextAccessor.HttpContext.User);

            if (user.Identity.Name == null)
            {
                await _signInManager.SignOutAsync();
                var redirect = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "login", Autologout = "yes" }));
                filterContext.Result = redirect;
            }
            if (user.Claims.Count() > 0)
            {
                if (this.StaticAuthorization) authorized = user.HasClaim(m => m.Value == this.Key);
            }
            else
            {
                await _signInManager.SignOutAsync();
                var redirect = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "login", Autologout = "yes" }));
                filterContext.Result = redirect;
            }
            if (!authorized)
            {
                result.Exception = new Exception("User does not have authorization for this action.");
                result.Success = false;

                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = (IActionResult?)result;
                    //filterContext.Result = new JsonResult(result);
                    return;
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    var redirect = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "login", nopermission = Guid.NewGuid() }));
                    filterContext.Result = redirect;
                }
            }


        }

    }
}
