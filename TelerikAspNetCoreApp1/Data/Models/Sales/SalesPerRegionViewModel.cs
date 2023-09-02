using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelerikAspNetCoreApp1.Data.Models.Sales;
public class SalesPerRegionViewModel
{
    public string Country { get; set; }
    public string Color { get; set; }
    public double Completed { get; set; }
    public double NotCompleted { get; set; }
}
