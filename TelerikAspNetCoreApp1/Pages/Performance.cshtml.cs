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
using TelerikAspNetCoreApp1.Data.Models.Sales;

namespace TelerikAspNetCoreApp1.Pages;
[Authorize]
public class PerformanceModel : PageModel
{
    private IDataService _dataService;

    public PerformanceModel(IDataService dataService)
    {
        _dataService = dataService;
    }
    [BindProperty]
    public int Target { get; set; }
    [BindProperty]
    public int TotalSalesPerWeekCount { get; set; }
    [BindProperty]
    public int LastMonthSalesCount { get; set; }
    [BindProperty]
    public int LastThreeMonthsSalesCount { get; set; }
    public void OnGet()
    {
        Target = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 1, 1))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .Count();
        TotalSalesPerWeekCount = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 12, 23))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .Count();
        LastMonthSalesCount = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 11, 30))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .Count();
        LastThreeMonthsSalesCount = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 9, 30))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .Count();

    }

    public JsonResult OnPostGetSalesPreferences()
    {
        var salesByPaymentType = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 1, 7))
                            .Where(sale => sale.TransactionDate < new DateTime(2019, 1, 14))
                            .ToList()
                            .GroupBy(sale => new { sale.PaymentType, sale.TransactionDate.Date, sale.TransactionDate.DayOfWeek })
                            .Select(group => new
                            {
                                DayOfWeek = group.Key.DayOfWeek,
                                PaymentType = group.Key.PaymentType,
                                Min = group.Where(sale => sale.PaymentType == group.Key.PaymentType).Min(sale => sale.Amount),
                                Max = group.Where(sale => sale.PaymentType == group.Key.PaymentType).Max(sale => sale.Amount)
                            }).OrderBy(o => o.DayOfWeek);

        return new JsonResult(salesByPaymentType.ToList());
    }

    public JsonResult OnPostGetSales()
    {
        var salesByRegion = _dataService.GetSales()
                            .Where(sale => sale.TransactionDate > new DateTime(2019, 1, 1))
                            .Where(sale => sale.TransactionDate < new DateTime(2020, 1, 1))
                            .GroupBy(sale => new { sale.Region, sale.TransactionDate.Year, sale.TransactionDate.Month })
                            .Select(group => new SalesByDateViewModel
                            {
                                Date = new DateTime(group.Key.Year, group.Key.Month, 1),
                                Region = group.Key.Region,
                                Sum = group.Sum(sale => sale.Amount)
                            });

        return new JsonResult(salesByRegion.ToList());
    }

    public JsonResult OnPostGetSalesPerRegion(string region)
    {
        var target = region == "EMEA" ? 1200 : 1400;
        var salesPerRegion = _dataService.GetSales()
            .Where(sale => region == "EMEA" ? sale.Region == region : sale.Region != region)
            .GroupBy(sale => new { sale.Country, sale.Region })
            .Select(group => new SalesPerRegionViewModel()
            {
                Country = group.Key.Country,
                Completed = group.Sum(sale => sale.Amount) / target * 100,
                NotCompleted = (target - group.Sum(sale => sale.Amount)) / target * 100
            })
            .OrderByDescending(x => x.Completed)
            .Take(5)
            .ToList();

        var colors = new string[] { "#FF6358", "#FFD246", "#AA46BE", "#2D73F5", "#28B4C8" };
        for (int i = 0; i < salesPerRegion.Count(); i++)
        {
            salesPerRegion[i].Color = colors[i];
        }

        return new JsonResult(salesPerRegion.ToList());
    }
    public JsonResult OnPostGetSalesPerGroup()
    {
        var salesPerGroup = _dataService.GetSales()
            .GroupBy(sale => new { sale.ProductGroup })
            .Select(group => new
            {
                Group = group.Key.ProductGroup,
                Amount = group.Where(sale => sale.ProductGroup == group.Key.ProductGroup).Sum(sale => sale.Amount),
                Explode = group.Key.ProductGroup == "Consumer Food" ? true : false
            });
        return new JsonResult(salesPerGroup.ToList());
    }
}
