using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using AdminPortalV8.Services;
using AdminPortalV8.Data.ExtendedIdentity;
using EPatrol.Services;
using AdminPortalV8.Models.Epatrol;

namespace AdminPortalV8.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IMailService _MailService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IMailService mailService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _MailService = mailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Username { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.Username);

                if (user == null || user.Email != Input.Email)
                {
                    // For security, you could show the same message regardless of match
                    ModelState.AddModelError(string.Empty, "Invalid username or email.");
                    return Page();
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                var url = HtmlEncoder.Default.Encode(callbackUrl);

                EmailModel mailRequest = new EmailModel();
                mailRequest.ToEmail = Input.Email;
                mailRequest.Subject = "[Visitor Vehicle Management System] - Reset Password";
                mailRequest.Body = "Please reset your password by <a href='" + url + "'>clicking here</a>.";

                await _MailService.SendEmailAsync(mailRequest);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}