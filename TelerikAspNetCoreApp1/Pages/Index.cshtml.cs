using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelerikAspNetCoreApp1.Data;
using TelerikAspNetCoreApp1.Data.Models.Employees;
using TelerikAspNetCoreApp1.Data.Models.Sales;

namespace TelerikAspNetCoreApp1.Pages;
[Authorize]
public class IndexModel : PageModel
{
    private IDataService _dataService;

    public IndexModel(IDataService dataService)
    {
        _dataService = dataService;
    }


    public void OnGet()
    {

    }

    public JsonResult OnPostGetSales(int type)
    {
        var salesByRegion = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 1, 1))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .GroupBy(sale => new { sale.Region, sale.TransactionDate.Year, sale.TransactionDate.Month })
                            .Select(group => new SalesByDateViewModel
                            {
                                Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                                Region = group.Key.Region,
                                Sum = type != 1 ? group.Sum(sale => sale.Amount) : group.Count()
                            });

        return new JsonResult(salesByRegion.ToList());
    }

    public JsonResult OnPostRead([DataSourceRequest] DataSourceRequest request, int team)
    {
        var employees = _dataService.GetEmployees().AsQueryable();

        if (team != 1)
        {
            employees = employees.Where(x => x.JobTitle.Contains("Engineer"));
        }

        var data = employees.Select(x => new EmployeeViewModel()
        {
            Id = x.Id,
            FullName = x.FullName,
            JobTitle = x.JobTitle,
            Rating = x.Rating,
            Budget = x.Budget
        }).ToList();
        return new JsonResult(data.ToDataSourceResult(request));
    }
}
