using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
public class RegisterModel : PageModel
{
    private IDataService _dataService;
    public RegisterModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool AgreeToTerms { get; set; }
    }
    public void OnGet(string returnUrl = null)
    {
        if (Input == null)
        {
            Input = new InputModel();
        }
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var userExists = _dataService.GetUsers().Exists(x => x.Email == Input.Email);
            if (userExists)
            {
                ModelState.AddModelError("Email", "You have already registerd with this email, proceed to Login");
                return Page();
            }

            var passwordHasher = new PasswordHasher<string>();
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Company = Input.Company,
                FullName = Input.FullName,
                AgreeToTerms = Input.AgreeToTerms,
                HashedPassword = passwordHasher.HashPassword(null, Input.Password)
            };
            _dataService.AddUser(user);

            var claims = new List<Claim>
                    {
                        new Claim("Email", user.Email),
                        new Claim("Name", user.FullName)
                    };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return LocalRedirect(returnUrl);
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
