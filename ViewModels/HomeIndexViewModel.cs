using CubeflakesRealtorDemo.Models;

namespace CubeflakesRealtorDemo.ViewModels;

public class HomeIndexViewModel
{
    public string HeroEyebrow { get; set; } = "";
    public string HeroTitle { get; set; } = "Discover homes that feel designed for modern living.";
    public string HeroSubtitle { get; set; } = "A polished realtor demo built with ASP.NET Core MVC, Bootstrap 5, and a reusable shared component system.";
    public List<PropertyModel> FeaturedProperties { get; set; } = new();
}
