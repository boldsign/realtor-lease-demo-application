using System.Text.Json;
using System.Threading.Tasks;
using BoldSign.Api;
using BoldSign.Api.Model;
using BoldSign.Model;
using System.Collections.Generic;
using CubeflakesRealtorDemo.Models;
using Microsoft.AspNetCore.Mvc;
using BoldSign.Model.Webhook;
using Microsoft.Extensions.Caching.Distributed;

namespace CubeflakesRealtorDemo.Controllers
{
    [Route("signing")]
    public class SigningController : Controller
    {
        private readonly ILogger<SigningController> _logger;
        private static readonly ApiClient apiClient = new ApiClient("https://api.boldsign.com",Environment.GetEnvironmentVariable("REALTORAPIKEY"));
        private readonly string templateId = Environment.GetEnvironmentVariable("REALTORTEMPLATEID");
        private readonly DocumentClient documentClient = new DocumentClient(apiClient);
        private readonly TemplateClient templateClient = new TemplateClient(apiClient);
        private readonly IDistributedCache _cache;
        
        public SigningController(ILogger<SigningController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("/booking/sign")]
        public IActionResult Sign()
        {
            if (TempData["BookingData"] == null)
            {
                return RedirectToAction("Index", "Booking");
            }

            var bookingJson = (string)TempData["BookingData"]!;
            var model = JsonSerializer.Deserialize<BookingModel>(bookingJson)!;

            // Render a loading page that auto-submits the form to POST Sign,
            // which will create the document and generate the embedded signing link.
            return View("~/Views/Booking/SignLoading.cshtml", model);
        }

        [HttpPost("/booking/sign")]
        [IgnoreAntiforgeryToken]
        // Create EmbedSignLink for the document
        public async Task<IActionResult> Sign(BookingModel templateDetails)
        {
            templateDetails.TemplateId = this.templateId;

            var documentDetails = new SendForSignFromTemplate()
            {
                Title = "Cubeflakes - Realtor Lease Demo",
                TemplateId = templateDetails.TemplateId,
                BrandId = Environment.GetEnvironmentVariable("REALTORBRANDID"),
                DisableEmails = true,
                Roles = new List<Roles>()
                {
                    new Roles
                    {
                        SignerName = templateDetails.FullName,
                        SignerEmail = templateDetails.Email,
                        RoleIndex = 1,
                        SignerType = SignerType.Signer,
                        ExistingFormFields = new List<ExistingFormField>()
                        {
                            new ExistingFormField() { Id = "txtOwner",         Value = "Scott Bennett" },
                            new ExistingFormField() { Id = "txtOwnerName",     Value = "Scott Bennett" },
                            new ExistingFormField() { Id = "txtTenant",        Value = templateDetails.FullName ?? string.Empty },
                            new ExistingFormField() { Id = "txtTenantName",    Value = templateDetails.FullName ?? string.Empty },
                            new ExistingFormField() { Id = "txtHouseAddress",  Value = templateDetails.Address ?? string.Empty },
                            new ExistingFormField() { Id = "txtAvailableFrom", Value = templateDetails.MoveInDate?.ToString("MMMM d, yyyy") ?? string.Empty },
                            new ExistingFormField() { Id = "txtLeaseTerm",     Value = templateDetails.LeaseTerm.ToString() + " months" },
                            new ExistingFormField() { Id = "txtMonthlyRent",   Value = "$" + templateDetails.MonthlyRent.ToString("F2") },
                            new ExistingFormField() { Id = "txtSecurityDeposit", Value = "$" + templateDetails.MonthlyRent.ToString("F2") },
                            new ExistingFormField() { Id = "txtPropertyName",  Value = templateDetails.PropertyName ?? string.Empty },
                            new ExistingFormField() { Id = "txtPhoneNumber",   Value = templateDetails.Phone ?? string.Empty },
                            new ExistingFormField() { Id = "txtTenantEmail",   Value = templateDetails.Email ?? string.Empty }
                        }
                    }
                }
            };

            // Create document from Template with the prefilled form fields
            DocumentCreated? documentCreated = null;
            try
            {
                documentCreated = await this.templateClient
                    .SendUsingTemplateAsync(sendForSignFromTemplate: documentDetails).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BoldSign SendUsingTemplate failed: {Message}", ex.Message);
            }

            // Guard: if document creation failed, show an error view instead of crashing
            if (documentCreated == null)
            {
                ViewBag.ErrorMessage = "We were unable to create the lease document at this time. Please try again.";
                return View("~/Views/Booking/Sign.cshtml", templateDetails);
            }

            templateDetails.DocumentId = documentCreated.DocumentId;

            // Persist booking data in cache so ThankYou can retrieve it by documentId
            await _cache.SetStringAsync(
                $"booking:{documentCreated.DocumentId}",
                JsonSerializer.Serialize(templateDetails),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                });



            // Create embedded signing URL from the document
            EmbeddedSigningLink embeddedSignUrl = this.documentClient.GetEmbeddedSignLink(
                documentId: templateDetails.DocumentId,
                signerEmail: templateDetails.Email,
                DateTime.Now.AddDays(30),
                redirectUrl: $"{this.Request.Scheme}://{this.Request.Host}/booking/responses");

            templateDetails.SignLink = embeddedSignUrl.SignLink; // Loaded into the iframe
            return View("~/Views/Booking/Sign.cshtml", templateDetails);
        }

        [HttpGet("status/{id}")]
        public async Task<IActionResult> GetStatus(string id)
        {
            // Check if the document exists in the cache and its status is "completed"
            var status = await _cache.GetStringAsync(id);
            if (status == null || status != "completed")
            {
                // Document does not exist in the cache or its status is not "completed"
                return NotFound();
            }

            // Document has been completed and is ready for download
            return Ok();
        }
        
         [HttpPost("/signing/Webhook")]
        [IgnoreAntiforgeryToken]
        // Action for Webhook
        public async Task<IActionResult> Webhook()
        {
            var sr = new StreamReader(this.Request.Body);
            var json = await sr.ReadToEndAsync();

            if (this.Request.Headers[WebhookUtility.BoldSignEventHeader] == "Verification")
            {
                return this.Ok();
            }

            // TODO: Update your webhook secret key
            var SECRET_KEY = Environment.GetEnvironmentVariable("REALTORWEBHOOKKEY");
            if (string.IsNullOrEmpty(SECRET_KEY))
            {
                _logger.LogError("Webhook secret key is not configured");
                return this.BadRequest("Webhook secret key is not configured.");
            }
            try
            {
                WebhookUtility.ValidateSignature(json, this.Request.Headers[WebhookUtility.BoldSignSignatureHeader], SECRET_KEY);
            }
            catch (BoldSignSignatureException ex)
            {
                _logger.LogError(ex, "Webhook signature validation failed");

                return this.Forbid();
            }

            var eventPayload = WebhookUtility.ParseEvent(json);
            var doc = eventPayload.Data as DocumentEvent;
            if (eventPayload.Event.EventType == WebHookEventType.Completed && doc != null)
            {
                _logger.LogInformation("Signing process completed for document {DocumentId}", doc.DocumentId);
                // Store the results in the cache with the same document ID
                _cache.SetString(doc.DocumentId, "completed", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
                // Redirect to the ThankYou page with the documentId in the route
                return RedirectToAction("ThankYou", "Booking", new { documentId = doc.DocumentId });
            }
            return this.Ok();
        }

        // Download the document using DocumentId
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(string id)
        {
            var document =await documentClient.DownloadDocumentAsync(id).ConfigureAwait(false);
            var contentType = "application/pdf"; // Set the content type of the file
            var fileName = "Copy_RealtorLeaseDemo.pdf"; // Set the file name
            Response.Headers["Content-Disposition"] = "Attachment; filename=" + fileName;
            return File(document, contentType, fileName);
        }
    }
}
