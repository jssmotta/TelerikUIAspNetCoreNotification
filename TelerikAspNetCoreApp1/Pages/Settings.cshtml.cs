using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelerikAspNetCoreApp1.Data;
using TelerikAspNetCoreApp1.Data.Models;

namespace TelerikAspNetCoreApp1.Pages;
[Authorize]
public class SettingsModel : PageModel
{
    private IDataService _dataService;
    public SettingsModel(IDataService dataService)
    {
        _dataService = dataService;
    }
    [BindProperty]
    public UserDetailsModel UserDetails { get; set; }
    [BindProperty]
    public UserInformationModel UserInformation { get; set; }
    public void OnGet()
    {
        ClaimsPrincipal currentUser = User;

        var loggedUser = _dataService.GetUsers().First(u => u.Email == User.FindFirst(c => c.Type == "Email")?.Value);
        if (UserDetails == null)
        {
            UserDetails = new UserDetailsModel()
            {
                Email = loggedUser.Email,
                Username = loggedUser.Email,
                Phone = loggedUser.PhoneNumber ?? "112345678901",
                Nickname = loggedUser.FullName
            };
        }
        if (UserInformation == null)
        {
            UserInformation = new UserInformationModel()
            {
                Website = "https://www.telerik.com/",
                WorkPhone = loggedUser.PhoneNumber ?? "112345678901",
                Country = "1",
            };
        }
    }
    public class UserDetailsModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Nickname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class UserInformationModel
    {
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string WorkPhone { get; set; }
    }
}
