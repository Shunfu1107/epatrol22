using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Filters;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers.Database;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;
using System.Web.Helpers;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Helpers
{
    public class AuthManager
    {
        private const string KEY_ID_SESSION = "~.idty";
        private static AuthService service;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public static UserManager<ApplicationUser> UserManager { get; set; }
        public static PermissionManager PermissionManager { get; private set; }
        public static string AuthenticationType { get; private set; }
        //public static ClaimsIdentity Identity
        //{
        //    get
        //    {
        //        var identityObj = HttpContext.Current.Session[KEY_ID_SESSION];
        //        var identity = identityObj as ClaimsIdentity;
        //        if (identity == null)
        //        {
        //            identity = new ClaimsIdentity();
        //        }
        //        return identity;
        //    }
        //    private set
        //    {
        //        HttpContext.Current.Session[KEY_ID_SESSION] = value;
        //    }
        //}
        static AuthManager()
        {
            
            PermissionManager = new PermissionManager();
            //service = new AuthService();
        }

        public static void Initialize()
        {
            PermissionManager.Initialize();
            //AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie;

            //CreateDatabase();

            //Add your table creation here
            //CreateUserTable();
            //CreateModuleTable();
            //CreateModuleOrganizeTable();
            //CreateAcccessLevelTable();
            //CreateRoleTable();
            //CreateRoleAccessTable();
            //CreateRoleProfileTable();
            //CreateAccessLevelDetailTable();
            //CreateUserProfileTable();
            //CreateAccessLevelExclusiveTable();
            //CreateUserPasswordHistoryTable();

            //Create data
            //CreateDefaultUser();
            //CreateDefaultModule();
            //CreateDefaultAccessLevel();
            //CreateDefaultRole();
            //CreateDefaultRoleAccess();
            //CreateDefaultRoleProfiles();
            //CustomCreateModuleDetail();
            //CustomCreateAccessLevelDetail();
        }

        //public static IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        //return baseContext.GetOwinContext().Authentication;
        //        return HttpContext.Current.Request.GetOwinContext().Authentication;
        //    }
        //}

        //public static async Task SignInAsync(AppUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identityCookies = await UserManager.CreateIdentityAsync(user, AuthenticationType);
        //    var identitySession = identityCookies.Clone();
        //    var profiles = service.GetUserProfiles(user.UserID);

        //    string cookieToken, formToken;
        //    AntiForgery.GetTokens(null, out cookieToken, out formToken);
        //    if (HttpContext.Current.Response.Cookies.AllKeys.Contains("__RequestVerificationToken"))
        //        HttpContext.Current.Response.Cookies["__RequestVerificationToken"].Value = cookieToken;
        //    else
        //        HttpContext.Current.Response.Cookies.Add(new HttpCookie("__RequestVerificationToken", cookieToken));

        //    if (profiles != null && !profiles.Pvid.Equals(0))
        //    {
        //        //DEFAULT ROLE
        //        var tempPermissionOwnership = new Dictionary<string, object>(3);
        //        var roleprofiles = profiles.RoleProfiles;
        //        for (var i = 0; i < roleprofiles.Count; i++)
        //        {
        //            var role = roleprofiles[i].Role;
        //            var roleAccess = role.AccessList;

        //            if (!role.IsActive) continue;
        //            if (role.EndActiveDate < DateTime.Today || role.StartActiveDate.Date > DateTime.Today) continue;

        //            for (var x = 0; x < roleAccess.Count; x++)
        //            {
        //                var roleaccess = roleAccess[x];
        //                var access = roleaccess.AccessLevel;
        //                for (var y = 0; y < access.AccessDetails.Count; y++)
        //                {
        //                    var permission = access.AccessDetails[y];

        //                    /*CONSIDER AGAIN*/
        //                    if (!roleaccess.IdentityModulePvid.Equals(permission.IdentityModulePvid)) continue;


        //                    /*STATIC AUTHORIZATION*/
        //                    if (!tempPermissionOwnership.Keys.Contains(permission.PermissionKey))
        //                    {
        //                        identitySession.AddClaim(new System.Security.Claims.Claim(AuthClaimTypes.Permission, permission.PermissionKey));
        //                        tempPermissionOwnership.Add(permission.PermissionKey, null);
        //                    }
        //                }
        //            }
        //        }


        //        //OVERRIDE ROLE BY EXLUSIVE ROLE
        //        for (var i = 0; i < profiles.AccessLevelExclusiveList.Count; i++)
        //        {
        //            var permission = profiles.AccessLevelExclusiveList[i];
        //            var staticClaim = identitySession.Claims.FirstOrDefault(m => m.Value == permission.PermissionKey);
        //            if (permission.Accessible)
        //            {
        //                identitySession.AddClaim(new System.Security.Claims.Claim(AuthClaimTypes.Permission, permission.PermissionKey));
        //            }

        //            if (staticClaim != null && !permission.Accessible)
        //            {
        //                identitySession.RemoveClaim(staticClaim);
        //            }

        //        }
        //    }

        //    if (isPersistent)
        //    {
        //        //will create a cookie with expiration time equal to ExpireTimeSpan you set up in Startup.cs
        //        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identityCookies);
        //    }
        //    else
        //    {
        //        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.Now.AddHours(1) }, identityCookies);
        //    }

        //    Identity = identitySession;
        //}

        //public static void SignOut()
        //{
        //    AuthenticationManager.SignOut();
        //    HttpContext.Current.Session.Abandon();
        //    HttpContext.Current.Session.Clear();
        //    HttpContext.Current.Session.RemoveAll();
        //    HttpContext.Current.Response.Cookies.Remove(".AspNet.ApplicationCookie");
        //    HttpContext.Current.Response.Cookies.Remove("ASP.NET_SessionId");

        //    if (HttpContext.Current.Response.Cookies["__RequestVerificationToken"] != null)
        //    {
        //        HttpContext.Current.Response.Cookies["__RequestVerificationToken"].Value = string.Empty;
        //    }
        //    if (HttpContext.Current.Response.Cookies["ASP.NET_SessionId"] != null)
        //    {
        //        //HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
        //        //SessionIDManager manager = new SessionIDManager();
        //        //string newID = manager.CreateSessionID(HttpContext.Current);
        //        //bool redirected = false;
        //        //bool isAdded = false;
        //        //manager.SaveSessionID(HttpContext.Current, newID, out redirected, out isAdded);
        //        //HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Values.Add("SameSite", "Lax");
        //        HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
        //    }

        //}

        
    }
}
