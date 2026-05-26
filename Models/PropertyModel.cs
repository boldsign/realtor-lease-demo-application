namespace CubeflakesRealtorDemo.Models;

public class PropertyModel
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public decimal SecurityDeposit { get; set; }
    public int LeaseTerm { get; set; }          // months
    public DateOnly AvailableFrom { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Parking { get; set; }
    public string PetPolicy { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}
