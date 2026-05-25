using CubeflakesRealtorDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CubeflakesRealtorDemo.Controllers;

[Route("properties")]
public class PropertyController : Controller
{
    [HttpGet("{slug}")]
    public IActionResult Details(string slug)
    {
        var property = PropertyCatalog.GetBySlug(slug) ?? PropertyCatalog.FeaturedProperties.First();

        return View(property);
    }
}
