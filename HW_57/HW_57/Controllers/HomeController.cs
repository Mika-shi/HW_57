using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HW_57.Models;
using HW_57.Models.Enums;
using HW_57.ViewModels;

namespace HW_57.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}