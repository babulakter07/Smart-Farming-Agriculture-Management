using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firming_Solution.Web.Models;

namespace Firming_Solution.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Weather: static representative data (real integration would call a weather API)
        ViewBag.WeatherData = new[]
        {
            new { City = "Dhaka",      Temp = 32, Condition = "Sunny",   Icon = "fa-sun",           Humidity = 78, Wind = 12 },
            new { City = "Chittagong", Temp = 30, Condition = "Cloudy",  Icon = "fa-cloud",         Humidity = 82, Wind = 18 },
            new { City = "Rajshahi",   Temp = 35, Condition = "Sunny",   Icon = "fa-sun",           Humidity = 55, Wind = 8  },
            new { City = "Khulna",     Temp = 31, Condition = "Rainy",   Icon = "fa-cloud-showers-heavy", Humidity = 88, Wind = 22 },
        };
        return View();
    }

    public IActionResult Weather()
    {
        ViewData["Title"] = "Weather Forecast";
        ViewBag.WeatherData = new[]
        {
            new { City = "Dhaka",      Temp = 32, Condition = "Sunny",   Icon = "fa-sun",           Humidity = 78, Wind = 12, Description = "Clear skies, good for outdoor farm work." },
            new { City = "Chittagong", Temp = 30, Condition = "Cloudy",  Icon = "fa-cloud",         Humidity = 82, Wind = 18, Description = "Overcast skies, moderate winds." },
            new { City = "Rajshahi",   Temp = 35, Condition = "Sunny",   Icon = "fa-sun",           Humidity = 55, Wind = 8,  Description = "Hot and dry, ensure animals have water." },
            new { City = "Khulna",     Temp = 31, Condition = "Rainy",   Icon = "fa-cloud-showers-heavy", Humidity = 88, Wind = 22, Description = "Heavy rain expected, protect feed stocks." },
            new { City = "Sylhet",     Temp = 28, Condition = "Rainy",   Icon = "fa-cloud-rain",    Humidity = 92, Wind = 15, Description = "Persistent rainfall, flooding risk in low areas." },
            new { City = "Barisal",    Temp = 29, Condition = "Cloudy",  Icon = "fa-cloud-sun",     Humidity = 85, Wind = 20, Description = "Partly cloudy, chance of afternoon showers." },
        };
        return View();
    }

    public IActionResult Diseases()
    {
        ViewData["Title"] = "Common Animal Diseases";
        return View();
    }

    public IActionResult Fertilizer()
    {
        ViewData["Title"] = "Fertilizer Guide";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
