using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using TelerikAspNetCoreApp1.Data.Models;
using TelerikAspNetCoreApp1.Data.Models.Employees;
using TelerikAspNetCoreApp1.Data.Models.Sales;

namespace TelerikAspNetCoreApp1.Data;
public interface IDataService
{
    IWebHostEnvironment WebHostEnvironment { get; set; }

    ApplicationUser AddUser(ApplicationUser user);
    List<Employee> GetEmployees();
    IEnumerable<Sale> GetSales();
    List<Team> GetTeams();
    List<ApplicationUser> GetUsers();
}