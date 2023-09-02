using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelerikAspNetCoreApp1.Data.Models;
public class ApplicationUser
{
    public string FullName { get; set; }
    public string Company { get; set; }
    public bool AgreeToTerms { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public string Nickname { get; set; }
    public string HashedPassword { get; set; }
}
