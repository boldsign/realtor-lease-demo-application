    // Listen for postMessage events from the BoldSign embedded signing iframe
    window.addEventListener("message", function (e) {
        console.log("BoldSign event received:", e.data);

        if (e.data && e.data.action === "onDocumentSigned" && e.data.documentId) {
            // Redirect to the thank-you URL with the BoldSign documentId in the path
            window.location.href = "/booking/thank-you/" + e.data.documentId;
        }
    }, false);