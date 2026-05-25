using System.Text.Json;
using System.Text.RegularExpressions;
using CubeflakesRealtorDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CubeflakesRealtorDemo.Controllers;

[Route("booking")]
public class BookingController : Controller
{
    private readonly IDistributedCache _cache;

    public BookingController(IDistributedCache cache)
    {
        _cache = cache;
    }

   
    // Entry point from Details page — stores property data in cache, redirects to clean URL
    [HttpPost("start")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(
        string? PropertyName,
        string? ImageUrl,
        string? Address,
        decimal MonthlyRent,
        int Bedrooms,
        int Bathrooms,
        int Parking,
        int LeaseTerm,
        string? PetPolicy,
        DateOnly AvailableFrom)
    {
        // Build a human-readable session key: "{property-slug}-{8-char hex}"
        // e.g. "modern-2bhk-apartment-a1b2c3d4"
        var nameSlug = Regex.Replace(
            (PropertyName ?? "property").ToLowerInvariant().Replace(" ", "-"),
            @"[^a-z0-9\-]", "");
        var shortId = Guid.NewGuid().ToString("N")[..8];
        var id = $"{nameSlug}-{shortId}";

        var model = new BookingModel
        {
            PropertyName  = PropertyName ?? "Sunset Villa",
            ImageUrl      = ImageUrl     ?? "",
            Address       = Address      ?? "",
            MonthlyRent   = MonthlyRent,
            Bedrooms      = Bedrooms,
            Bathrooms     = Bathrooms,
            Parking       = Parking,
            LeaseTerm     = LeaseTerm,
            PetPolicy     = PetPolicy    ?? "",
            AvailableFrom = AvailableFrom,
            FullName      = "Jane Smith",
            Email         = "jane@example.com",
            Phone         = "+1 (415) 555-0134",
            MoveInDate    = AvailableFrom != default
                                ? AvailableFrom.ToDateTime(TimeOnly.MinValue)
                                : DateTime.Today
        };

        await _cache.SetStringAsync(
            $"booking_property:{id}",
            JsonSerializer.Serialize(model),
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

        return RedirectToAction("Index", new { id });
    }

    [HttpGet("{id?}")]
    public async Task<IActionResult> Index(string? id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var json = await _cache.GetStringAsync($"booking_property:{id}");
            if (!string.IsNullOrEmpty(json))
            {
                var model = JsonSerializer.Deserialize<BookingModel>(json)!;
                return View(model);
            }
        }

        // No valid session — send back to property list
        return RedirectToAction("Index", "Home");
    }

   
    [HttpPost("{id?}")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(BookingModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        // ✅ Store model temporarily and redirect to signing controller
        TempData["BookingData"] = System.Text.Json.JsonSerializer.Serialize(model);
        return RedirectToAction("Sign", "Signing");
    }

    
    [HttpGet("responses")]
    public IActionResult Responses()
    {
        return View();
    }

    [HttpGet("thank-you/{documentId?}")]
    public async Task<IActionResult> ThankYou(string? documentId)
    {
        if (string.IsNullOrEmpty(documentId))
            return RedirectToAction("Index");

        var bookingJson = await _cache.GetStringAsync($"booking:{documentId}");
        if (string.IsNullOrEmpty(bookingJson))
            return RedirectToAction("Index");

        var model = JsonSerializer.Deserialize<BookingModel>(bookingJson)!;
        return View(model);
    }

}
