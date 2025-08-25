// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using AdminPortalV8.Libraries.ExtendedUserIdentity;
using AdminPortalV8.Services;
using AdminPortalV8.Helpers;
using AdminPortalV8.Models.User;

namespace AdminPortalV8.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUser _userService;
        private readonly IAuth _authService;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;


        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, IUser user, IAuth authService, 
            IClaimsTransformation claimsTransformation, UserObj usrObj, IGeneral general)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _userService = user;
            _authService = authService;
            _claimsTransformation = claimsTransformation;
            _usrObj = usrObj;
            _general = general;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            returnUrl ??= Url.Content("~/");

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(Input.UserName);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "This username does not exist!";
                        return Page();
                    }

                    AppUser userDets = _userService.GetUser(Input.UserName);

                    if (userDets.Active == false)
                    {
                        TempData["ErrorMessage"] = "Your account is inactive. Please contact administrator to activate.";
                        return Page();
                    }

                    //every login user need a user profile to use their access right.
                    var Userprofile = _userService.GetUserProfiles(new Libraries.ExtendedUserIdentity.Filters.UserProfilesByUserFilter()
                    {
                        UserPvid = user.Id
                    });
                    if (Userprofile == null)
                    {
                        _userService.AddUserProfiles(new AppUserProfiles()
                        {
                            UserPvid = user.Id
                        });
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var result2 = new JsonInfoResult();

                        try
                        {
                            var claimid = HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
                            var userprofile = default(AppUserProfiles);
                            var User = default(AppUser);

                            if (claimid == null) goto LB_ANONYMOUSE;
                            userprofile = _authService.GetUserProfiles(claimid.Value);
                            User = _authService.GetUserByID(long.Parse(claimid.Value));

                        LB_ANONYMOUSE:
                            result2.Data = new
                            {
                                User = user,
                                UserProfiles = Userprofile,
                                Permission = _general.GetAllContentPermissions()
                            };
                            result2.Success = true;
                            var json = JsonConvert.SerializeObject(result2);
                            HttpContext.Session.SetString("GetConfig", json);
                            HttpContext.Session.SetString("LoginUserId", user.Id.ToString());

                            _usrObj.user = User;
                            _usrObj.userProfiles = Userprofile;
                        }
                        catch (Exception ex)
                        {
                            result2.Exception = new Exception(ex.Message);
                            result2.Success = false;
                        }

                        string userPhoneNumber = user.PhoneNumber;
                        if (!string.IsNullOrEmpty(userPhoneNumber))
                        {
                            returnUrl = Url.Action("Index", "Dashboard");
                        }

                        if (userDets.FirstPasswordReset == false)
                        {
                            returnUrl = Url.Action("Index", "Profile");
                        }

                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        int attemp = 0;
                        if (HttpContext.Session.GetString("Attemp") != null)
                        {
                            attemp = Convert.ToInt32(HttpContext.Session.GetString("Attemp"));
                        }
                        attemp += 1;

                        if (attemp >= 5)
                        {
                            AppUser userDet = _userService.GetUser(Input.UserName);
                            _userService.UpdateActiveStatus(Convert.ToInt32(userDet.UserID), false);
                            TempData["ErrorMessage"] = "Too many failed attempts. Account has been deactivated. Please contact administrator.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Invalid username or password. Please try again.";
                        }

                        HttpContext.Session.SetString("Attemp", attemp.ToString());
                        return Page();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please enter both username and password.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt");
                TempData["ErrorMessage"] = "An error occurred during login. Please try again later.";
                return Page();
            }
        }
    }
}
