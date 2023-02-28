using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.BL.Models;
using Microsoft.AspNetCore.Identity;
using Demo.DAL.Extend;
using Demo.BL.Helper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        #region Registration (Sign Up)
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        IsAgree = model.IsAgree,
                        //PhotoName = FileUploader.Upload("/wwwroot/Files/Imgs", model.Img)
                    };
                   var result = await userManager.CreateAsync(user, model.Password);
                                await userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                       return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                return View(model);
            }
        }
        #endregion
        #region Login (Sign In)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
      
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "UserName Or Password Is Inncorrect");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                    }
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "UserName Or Password Is Incorrect");
                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                return View(model);
            }
        }
        #endregion
        #region LogOff (Sign Out)
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
           await signInManager.SignOutAsync();
           return RedirectToAction("Login");
        }
        #endregion
        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token });
                        MailSender.SendMail(new MailVM { Mail = model.Email, Title = "Password Reset Link", Message = ResetPasswordLink });
                        RedirectToAction("ConfirmForgetPassword");
                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                return View(model);
            }
        }
        [HttpGet]
        public IActionResult ConfirmForgetPassword()
        {
            return View();
        }
        #endregion
        #region Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string Email, string Token)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var result = await userManager.ResetPasswordAsync(user, model.token, model.Password);
                        if (result.Succeeded)
                        {
                            RedirectToAction("ConfirmResetPassword");
                        }
                        else
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("", item.Description);
                            }
                        }
                    }
                }
                return View(model);
            }
            catch (Exception)
            {

                return View(model);
            }
        }
        [HttpGet]
        public IActionResult ConfirmResetPassword()
        {
            return View();
        }
        #endregion
    }
}
