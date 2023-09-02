using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelerikAspNetCoreApp1.Data;
using TelerikAspNetCoreApp1.Data.Models;

namespace TelerikAspNetCoreApp1.Areas.Login.Pages;
[AllowAnonymous]
public class LoginModel : PageModel
{
    private IDataService _dataService;
    public LoginModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string returnUrl = null)
    {
        if (Input == null)
        {
            Input = new InputModel()
            {
                Email = "jaxons.danniels@company.com",
                Password = "User*123"

            };
        }

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");


        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = _dataService.GetUsers().First(x => x.Email == Input.Email);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<string>();
                var hashedPassword = passwordHasher.HashPassword(user.UserName, Input.Password);
                var verificationResult = passwordHasher.VerifyHashedPassword(null, hashedPassword, Input.Password);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                            {
                                new Claim("Email", user.Email),
                                new Claim("Name", user.FullName)
                            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return LocalRedirect(returnUrl);
                }
                else if (verificationResult == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("Password", "Invalid username or password");
                    return Page();
                }
            }
        }
        else
        {
            return Page();
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }

}
