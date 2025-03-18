using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DevoteWebsite.Models;

namespace DevoteWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Vote()
    {
        return RedirectToAction("DownloadCache", "File");
    }

    public IActionResult Store()
    {
        return RedirectToAction("Index", "Store");
    }

    public IActionResult Play()
    {
        return Content("Play Button");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
