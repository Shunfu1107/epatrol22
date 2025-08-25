using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AdminPortalV8.Helpers
{
    
    public interface IClaimsTransformations
    {
        Task<ClaimsPrincipal> AddPermission(AppUserProfiles userProfile);
    }

    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserObj _userObj;

        public ClaimsTransformer(IHttpContextAccessor httpContextAccessor, UserObj userObj)
        {
            _httpContextAccessor = httpContextAccessor;
            _userObj = userObj;
        }

        public Task<ClaimsPrincipal> AddPermission(AppUserProfiles userProfile)
        {
            try
            {
                var claims = new List<Claim>();
                foreach (var item in userProfile.RoleProfiles)
                {
                    foreach (var data in item.Role.AccessList)
                    {
                        foreach (var val in data.AccessLevel.AccessDetails)
                        {
                            claims.Add(new Claim("custom_" + val.PermissionKey, val.PermissionKey));
                        }
                    }
                }

                var identity = new ClaimsIdentity(claims, "custom");
                var principal = new ClaimsPrincipal(identity);
                _httpContextAccessor.HttpContext.User.AddIdentity(identity);

                return Task.FromResult(principal);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ClaimsPrincipal());
            }
            
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customClaims = new List<Claim>();

           if(_userObj.userProfiles != null)
            {
                try
                {
                    
                    foreach (var item in _userObj.userProfiles.RoleProfiles)
                    {
                        foreach (var roleProfiles in item.Role.AccessList)
                        {
                            foreach (var access in roleProfiles.AccessLevel.AccessDetails)
                            {
                                customClaims.Add(new Claim(access.PermissionKey, access.PermissionKey));
                            }
                        }
                    }

                    foreach (var item in _userObj.userProfiles.AccessLevelExclusiveList)
                    {
                        customClaims.Add(new Claim(item.PermissionKey, item.PermissionKey));
                    }
                }
                catch (JsonReaderException ex)
                {
                    // Handle the JSON deserialization exception
                }
            }

           
            

       
            var identity = (ClaimsIdentity)principal.Identity;
            identity.AddClaims(customClaims);

            return Task.FromResult(principal);
        }
    }
}
