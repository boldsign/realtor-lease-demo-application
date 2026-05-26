using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CubeflakesRealtorDemo.Models;

public class BookingModel
{
    // ✅ Property Details (ADD THESE)
    public string PropertyName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Parking { get; set; }
    public int LeaseTerm { get; set; }
    public string PetPolicy { get; set; } = string.Empty;
    public DateOnly AvailableFrom { get; set; }

    // ✅ Existing Fields (KEEP AS-IS)
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Move-in date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Move-in Date")]
    [CustomValidation(typeof(BookingModel), nameof(ValidateMoveInDate))]
    public DateTime? MoveInDate { get; set; }

    public static ValidationResult? ValidateMoveInDate(DateTime? date, ValidationContext context)
    {
        if (!date.HasValue) return ValidationResult.Success;
        var instance = (BookingModel)context.ObjectInstance;
        var earliest = instance.AvailableFrom != default
            ? instance.AvailableFrom.ToDateTime(TimeOnly.MinValue)
            : DateTime.Today;
        if (date.Value.Date < earliest.Date)
            return new ValidationResult($"Move-in date cannot be before {earliest:MMMM d, yyyy} (Available From).");
        return ValidationResult.Success;
    }

    // ── Signature fields (for embedded signing flow)
    public string SignatureData { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime? DateSigned { get; set; }
    
    // Link to an embedded signing session (iframe src)
    public string SignLink { get; set; } = string.Empty;
    // Template field mappings to prefill PDF template fields
    public Dictionary<string, string> TemplateFields { get; set; } = new Dictionary<string, string>();

    // BoldSign template/document identifiers (used during signing)
    public string TemplateId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
}
