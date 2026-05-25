using CubeflakesRealtorDemo.Models;
using CubeflakesRealtorDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CubeflakesRealtorDemo.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        var model = new HomeIndexViewModel
        {
            FeaturedProperties = PropertyCatalog.FeaturedProperties.ToList()
        };

        return View(model);
    }

    [HttpGet("about")]
    public IActionResult About()
    {
        return View();
    }
}
