namespace CubeflakesRealtorDemo.Models;

public static class PropertyCatalog
{
    private static readonly List<PropertyModel> Properties = new()
    {
        new()
        {
            Id = 1,
            Slug = "modern-2bhk-apartment",
            Name = "Modern 2BHK Apartment",
            Address = "101 Demo Street, Austin, TX",
            MonthlyRent = 1800,
            SecurityDeposit = 1800,
            LeaseTerm = 12,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            Bedrooms = 2,
            Bathrooms = 2,
            Parking = 1,
            PetPolicy = "No Pets",
            ImageUrl = "/images/house1.png",
            Description = "A stylish and fully modern 2-bedroom apartment in the heart of Austin. Enjoy open-plan living, premium finishes, and easy access to downtown dining, entertainment, and public transit. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "Open-plan kitchen",
                "In-unit washer & dryer",
                "Smart home thermostat",
                "Rooftop lounge access"
            }
        },
        new()
        {
            Id = 2,
            Slug = "maple-ridge-residence",
            Name = "Maple Ridge Residence",
            Address = "500 Sample Avenue, San Francisco, CA",
            MonthlyRent = 3250,
            SecurityDeposit = 3250,
            LeaseTerm = 12,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(40)),
            Bedrooms = 3,
            Bathrooms = 2,
            Parking = 1,
            PetPolicy = "Cats Only",
            ImageUrl = "/images/house2.png",
            Description = "Bright, modern interiors with skyline views, flexible living space, and polished finishes. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "Open-plan kitchen",
                "Floor-to-ceiling windows",
                "Smart home automation",
                "Rooftop lounge access"
            }
        },
        new()
        {
            Id = 3,
            Slug = "lakeside-family-home",
            Name = "Lakeside Family Home",
            Address = "202 Demo Lane, Austin, TX",
            MonthlyRent = 4850,
            SecurityDeposit = 4850,
            LeaseTerm = 12,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(25)),
            Bedrooms = 4,
            Bathrooms = 3,
            Parking = 2,
            PetPolicy = "Pets Allowed",
            ImageUrl = "/images/house3.png",
            Description = "A spacious home near parks and schools with warm finishes and a private backyard. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "Large private yard",
                "Dedicated laundry room",
                "Energy efficient appliances",
                "Quiet neighborhood"
            }
        },
        new()
        {
            Id = 4,
            Slug = "sunset-garden-villa",
            Name = "Sunset Garden Villa",
            Address = "303 Demo Boulevard, Miami, FL",
            MonthlyRent = 4100,
            SecurityDeposit = 4100,
            LeaseTerm = 12,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(70)),
            Bedrooms = 3,
            Bathrooms = 2,
            Parking = 1,
            PetPolicy = "No Pets",
            ImageUrl = "/images/house4.png",
            Description = "A bright villa-style retreat with elegant landscaping and a resort-inspired feel. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "Private patio",
                "Landscaped garden",
                "Walk-in closets",
                "Designer lighting"
            }
        },
        new()
        {
            Id = 5,
            Slug = "downtown-modern-loft",
            Name = "Downtown Modern Loft",
            Address = "404 Sample Drive, Seattle, WA",
            MonthlyRent = 2950,
            SecurityDeposit = 2950,
            LeaseTerm = 6,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(55)),
            Bedrooms = 2,
            Bathrooms = 2,
            Parking = 1,
            PetPolicy = "Pets Allowed",
            ImageUrl = "/images/house5.png",
            Description = "Minimalist loft living with city access, tall ceilings, and a refined modern layout. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "High ceilings",
                "Private balcony",
                "Pet-friendly building",
                "Fitness center access"
            }
        },
        new()
        {
            Id = 6,
            Slug = "oakridge-townhome",
            Name = "Oakridge Townhome",
            Address = "505 Demo Circle, Denver, CO",
            MonthlyRent = 3500,
            SecurityDeposit = 3500,
            LeaseTerm = 12,
            AvailableFrom = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            Bedrooms = 3,
            Bathrooms = 3,
            Parking = 2,
            PetPolicy = "Cats Only",
            ImageUrl = "/images/house6.png",
            Description = "A stylish townhome with flexible spaces, modern finishes, and easy commuting access. This content is presented for demo purposes only.",
            Features = new List<string>
            {
                "Private garage",
                "Flexible bonus room",
                "Modern kitchen island",
                "Community green space"
            }
        }
    };

    public static IReadOnlyList<PropertyModel> FeaturedProperties => Properties;

    public static PropertyModel? GetById(int id) => Properties.FirstOrDefault(p => p.Id == id);

    public static PropertyModel? GetBySlug(string slug) =>
        Properties.FirstOrDefault(p => string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase));
}