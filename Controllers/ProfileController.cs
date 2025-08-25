using AdminPortal.Libraries.ExtendedUserIdentity.Attributes;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AdminPortalV8.Data.ExtendedIdentity;
using Microsoft.AspNetCore.Identity;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using ZXing;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using AdminPortalV8.Services;
using AdminPortalV8.Models.User;

namespace AdminPortalV8.Controllers
{
    [ExtendedAuthorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserObj _usrObj;
        private readonly IGeneral _general;

        public ProfileController(UserManager<ApplicationUser> userManager, UserObj usrObj, IGeneral general)
        {
            _userManager = userManager;
            _usrObj = usrObj;
            _general = general;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ProfileViewModel();

            var usr = await _userManager.FindByIdAsync(_usrObj.user.Id);

            if (usr.FirstPasswordReset == false)
            {
                TempData["Message"] = "<script>bootbox.alert({centerVertical: true,size: \"small\",message: \"First time login. Please reset your password.\"});</script>";
            }

            model.Name = usr.Name;
            model.UserName = usr.UserName;
            model.Email = usr.Email;
            model.UserID = usr.Id;



            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usr = await _userManager.FindByIdAsync(model.UserID.ToString());

                    UserPasswordHistory PassHistory = _general.GetUserPasswordHistory(Convert.ToInt32(usr.Id));
                    bool check = false;
                    if (PassHistory == null)
                    {
                        UserPasswordHistory userPass = new UserPasswordHistory();
                        userPass.UserID = Convert.ToInt32(usr.Id);
                        userPass.PasswordHash1 = usr.PasswordHash;
                        userPass.PasswordDate1 = DateTime.Now;
                        _general.UpdatePasswordHistory(userPass);

                        var verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, usr.Password, model.Password);
                        if (verify.ToString().Contains("Success"))
                        {
                            check = true;
                        }
                    }
                    else
                    {
                        
                        var passwordhash = _userManager.PasswordHasher.HashPassword(usr, model.Password);
                        var verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash1, model.Password);
                        if (verify.ToString().Contains("Success"))
                        {
                            check = true;
                        }
                        else
                        {
                            verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash2, model.Password);
                            if (verify.ToString().Contains("Success"))
                            {
                                check = true;
                            }
                            else
                            {
                                verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash3, model.Password);
                                if (verify.ToString().Contains("Success"))
                                {
                                    check = true;
                                }
                                else
                                {
                                    verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash4, model.Password);
                                    if (verify.ToString().Contains("Success"))
                                    {
                                        check = true;
                                    }
                                    else
                                    {
                                        verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash5, model.Password);
                                        if (verify.ToString().Contains("Success"))
                                        {
                                            check = true;
                                        }
                                        else
                                        {
                                            verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash6, model.Password);
                                            if (verify.ToString().Contains("Success"))
                                            {
                                                check = true;
                                            }
                                            else
                                            {
                                                verify = _userManager.PasswordHasher.VerifyHashedPassword(usr, PassHistory.PasswordHash7, model.Password);
                                                if (verify.ToString().Contains("Success"))
                                                {
                                                    check = true;
                                                }
                                                else
                                                {
                                                    check = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        
                    }

                    if (check == true)
                    {
                        TempData["Message"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Password same with previous password. Change new password for secure.\"});</script>";
                        return View(model);
                    }

                    if (usr.PasswordHash != model.Password)
                    {
                        var asyncresult1 = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, usr, model.Password);
                      
                        var passwordhash = _userManager.PasswordHasher.HashPassword(usr, model.Password);
                        var newsecurityStamp = Guid.NewGuid().ToString();

                        var deletPass = await _userManager.RemovePasswordAsync(usr);

                        var a = await _userManager.AddPasswordAsync(usr, passwordhash);
                        usr.SecurityStamp = newsecurityStamp;
                        var b = await _userManager.UpdateSecurityStampAsync(usr);
                        usr.Password = passwordhash;
                        usr.PasswordHash = passwordhash;
                        var c = await _userManager.UpdateAsync(usr);

                        usr = await _userManager.FindByIdAsync(model.UserID.ToString());

                        await _general.UpdateFirstPassword(usr.Id);
                    }

                    



                    UserPasswordHistory userP = new UserPasswordHistory();
                    userP.UserID = Convert.ToInt32(usr.Id);
                    userP.PasswordHash1 = usr.Password;
                    userP.PasswordDate1 = DateTime.Now;
                    _general.UpdatePasswordHistory(userP);

                    TempData["Message"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Change password successfully\"});</script>";

                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = "<script>bootbox.alert({size: \"small\",title: \"Message\",message: \"Failed. Please try again\"});</script>";
            }
            
            return View(model);
        }
    }
}
