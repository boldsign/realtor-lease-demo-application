using CubeflakesRealtorDemo.Models;

namespace CubeflakesRealtorDemo.ViewModels;

public class PropertyDetailsViewModel
{
    public PropertyModel Property { get; set; } = new();
    public string InquiryEmail { get; set; } = "hello@cubeflakes.example";
}
