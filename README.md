# Cubeflakes Realtor Lease Demo

Welcome to the **Cubeflakes Realtor Lease Demo** — a full-featured ASP.NET Core web application that demonstrates a modern property leasing workflow with integrated electronic signatures powered by the [BoldSign API](https://boldsign.com/esignature-api/).

The demo is built from the perspective of **Cubeflakes**, a fictional real estate company, giving you a realistic, end-to-end example of how BoldSign can be embedded into a property rental platform.

---

## Overview

This demo showcases a complete rental journey — from browsing available properties to digitally signing a lease agreement — all within a single seamless web application.

**BoldSign API features used:**

- ✅ Template-based document generation
- ✅ Pre-filled form fields
- ✅ Embedded signing (iframe experience)
- ✅ Single signer workflow
- ✅ Custom branding
- ✅ Webhook for real-time signing status
- ✅ Signed document download

---

## Application Workflow

```
Browse Properties → View Property Details → Fill Booking Form → Sign Lease (Embedded) → Download Signed Lease
```

1. **Home** — Displays a curated catalog of 6 featured rental properties
2. **Property Details** — Shows full property info (rent, bedrooms, lease term, availability, features)
3. **Booking Form** — Tenant fills in personal details (name, email, phone, move-in date)
4. **Lease Signing** — A lease document is generated from a BoldSign template with pre-filled fields and presented as an embedded signing iframe
5. **Thank You / Download** — After signing is confirmed via webhook, tenant can download the signed PDF

---

## Project Structure

```
RealtorLeaseDemo/
├── Controllers/
│   ├── HomeController.cs          # Landing page & property listing
│   ├── PropertyController.cs      # Property details page
│   ├── BookingController.cs       # Booking form & session management
│   └── SigningController.cs       # BoldSign document creation, embedded sign link & webhook
├── Models/
│   ├── PropertyModel.cs           # Property data model
│   ├── PropertyCatalog.cs         # In-memory catalog of 6 demo properties
│   └── BookingModel.cs            # Tenant booking & lease form model
├── ViewModels/
│   ├── HomeIndexViewModel.cs      # Home page view model
│   ├── PropertyDetailsViewModel.cs
│   └── SectionTitleViewModel.cs
├── Views/
│   ├── Home/                      # Landing & About pages
│   ├── Property/                  # Property details
│   ├── Booking/
│   │   ├── Index.cshtml           # Booking form
│   │   ├── SignLoading.cshtml     # Auto-submit loading screen
│   │   ├── Sign.cshtml            # Embedded signing iframe
│   │   ├── Responses.cshtml       # Post-sign redirect handler
│   │   └── ThankYou.cshtml        # Confirmation & download
│   └── Shared/                    # Layout, navbar, footer, partials
├── wwwroot/
│   ├── css/                       # Stylesheets
│   ├── js/                        # Client-side scripts (sign, response, site)
│   ├── images/                    # Property images
│   └── fonts/
├── Program.cs                     # App startup & middleware
└── RealtorLeaseDemo.csproj        # Project file (targets .NET 8)
```

---

## Prerequisites

Before running this application, make sure you have:

- [.NET SDK 8.0 or later](https://dotnet.microsoft.com/download)
- A [BoldSign account](https://www.boldsign.com/) with API access
- A **BoldSign API Key**
- A **BoldSign Template ID** for the lease agreement document
- A **BoldSign Brand ID** (optional, for custom branding)
- A **Webhook Secret Key** for validating signing events

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd RealtorLeaseDemo
```

### 2. Configure API Credentials

Open `Controllers/SigningController.cs` and update the following values:

```csharp
private static readonly ApiClient apiClient = new ApiClient(
    "https://api.boldsign.com",
    "YOUR_BOLDSIGN_API_KEY"           // 🔑 Replace with your API key
);

private readonly string templateId = "YOUR_TEMPLATE_ID";   // 📄 Replace with your Template ID
```

Also update the **Brand ID** (optional) and **Webhook Secret Key** in the same file:

```csharp
BrandId = "YOUR_BRAND_ID",            // 🎨 Optional: your BoldSign brand
```

```csharp
var SECRET_KEY = "YOUR_WEBHOOK_SECRET_KEY";   // 🔐 Replace with your webhook secret
```

> **Recommendation:** For production use, move secrets to environment variables or a secrets manager instead of hardcoding them.

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

The application will start at `https://localhost:7179` (or `http://localhost:5183`).

---

## BoldSign Template Setup

To use this demo with your own BoldSign template, your lease document template should include the following **pre-fill form field IDs**:

| Field ID            | Description                        |
|---------------------|------------------------------------|
| `txtOwner`          | Property owner name                |
| `txtOwnerName`      | Property owner full name           |
| `txtTenant`         | Tenant name                        |
| `txtTenantName`     | Tenant full name                   |
| `txtHouseAddress`   | Property address                   |
| `txtAvailableFrom`  | Lease start / move-in date         |
| `txtLeaseTerm`      | Lease duration (e.g. "12 months")  |
| `txtMonthlyRent`    | Monthly rent amount                |
| `txtSecurityDeposit`| Security deposit amount            |
| `txtPropertyName`   | Property listing name              |
| `txtPhoneNumber`    | Tenant phone number                |
| `txtTenantEmail`    | Tenant email address               |

---

## Webhook Integration

The application listens for BoldSign webhook events at:

```
POST /signing/Webhook
```

- **Verification:** Responds `200 OK` to BoldSign's initial handshake
- **Signature Validation:** Validates all webhook payloads using your `SECRET_KEY`
- **Completion Event:** On `DocumentCompleted`, the document ID is cached and the tenant is redirected to the Thank You page

To configure the webhook in your BoldSign account, set the endpoint URL to:

```
https://<your-domain>/signing/Webhook
```

---

## Key Technologies

| Technology         | Purpose                                      |
|--------------------|----------------------------------------------|
| ASP.NET Core 8.0   | Web framework                                |
| BoldSign.Api 7.2.5 | Electronic signature integration             |
| Razor Views        | Server-side templating                       |
| Bootstrap 5        | Responsive UI framework                      |
| IDistributedCache  | In-memory session/document state management  |
| jQuery             | Client-side scripting                        |

---

## Security Considerations

✅ **Best Practices Implemented:**
- **Environment variables** for sensitive data (API keys, secrets)
- **Webhook signature validation** using BoldSign's `WebhookUtility.ValidateSignature()`
- **Input validation** on form fields with HTML5 attributes and client-side validation
- **Null reference checks** for safe environment variable handling
- **Exception handling** for API operations with user-friendly error messages

---

## Troubleshooting

**"We were unable to create the lease document at this time"**
- Verify your API key is valid and has `Send` permissions
- Confirm the Template ID exists in your BoldSign account
- Check logs for the underlying exception from `BoldSign.Api`

**Webhook signature validation failed (403 Forbidden)**
- Ensure `SECRET_KEY` in `SigningController.cs` exactly matches the webhook secret configured in BoldSign

**Embedded signing iframe does not load**
- Confirm `DisableEmails = true` is set (required for embedded signing)
- Verify the `redirectUrl` domain is whitelisted in your BoldSign account settings

**Booking session expired / blank form**
- The booking session is cached for 30 minutes — refresh and start from the property listing again

---

## Demo Properties

The application ships with **6 pre-configured demo properties** across major US cities:

| # | Property                  | City              | Rent/mo  | Beds | Lease    |
|---|---------------------------|-------------------|----------|------|----------|
| 1 | Modern 2BHK Apartment     | Austin, TX        | $1,800   | 2    | 12 months |
| 2 | Maple Ridge Residence     | San Francisco, CA | $3,250   | 3    | 12 months |
| 3 | Lakeside Family Home      | Austin, TX        | $4,850   | 4    | 12 months |
| 4 | Sunset Garden Villa       | Miami, FL         | $4,100   | 3    | 12 months |
| 5 | Downtown Modern Loft      | Seattle, WA       | $2,950   | 2    | 6 months  |
| 6 | Oakridge Townhome         | Denver, CO        | $3,500   | 3    | 12 months |

---

## Next Steps

- Customize the lease agreement template to match your property and legal requirements
- Add additional pre-fill form fields to the BoldSign template (e.g., pet policy, utilities, parking)
- Move API credentials to environment variables or a secrets manager for production use
- Deploy to Azure App Service, AWS, or your preferred hosting platform
- Integrate with a real property management backend or database for live listings
- Add multi-signer support (e.g., co-tenants or a property manager countersignature)

---

## References

- 📘 [BoldSign eSignature API](https://boldsign.com/esignature-api/)
- 📖 [BoldSign API Documentation](https://developers.boldsign.com/)
- 🚀 [BoldSign Live Demos](https://demos.boldsign.com/)
- 🆘 [BoldSign Support](https://support.boldsign.com/)

---

## License

This demo application is provided **as-is** for learning and demonstration purposes. All property data, company names, and tenant details are fictional.