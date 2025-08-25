using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class AddAccessRightViewModel
    {
       
        public AddAccessRightViewModel()
        {
            AccessLevelList = new List<AppAccessLevel>(1);
            ModuleList = new List<AppModule>(1);
            
        }

        [Required]
        [Display(Name = "Module")]
        public long ModulePvid { get; set; }

        [Required]
        public long RolePvid { get; set; }

        [Required]
        [Display(Name = "Access Level")]
        public long AccessLevelPvid { get; set; }

        public IList<AppModule> ModuleList { get; set; }
        public IList<AppAccessLevel> AccessLevelList { get; set; }

        public static implicit operator AppRoleAccess(AddAccessRightViewModel rights)
        {
            //ClaimsPrincipal principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            //ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
            var _httpContextAccessor = new HttpContextAccessor();

            //var identifier = principal.Claims.FirstOrDefault(m => m.ValueType == ClaimTypes.NameIdentifier);
            //var identifier = identity.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            var identifier = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            return new AppRoleAccess
            {
                GrantedByIdentityUserPvid = Convert.ToInt64(identifier.Value),
                GrantedDate = DateTime.Now,
                IdentityAccessLevelPvid = rights.AccessLevelPvid,
                IdentityModulePvid = rights.ModulePvid,
                IdentityRolePvid = rights.RolePvid
            };
        }
    }
}