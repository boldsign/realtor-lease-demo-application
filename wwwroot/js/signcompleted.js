window.addEventListener("load", function () {
    // Read the real BoldSign documentId from the data attribute embedded by the server
    const meta       = document.getElementById("thankyou-meta");
    const documentId = meta ? meta.dataset.documentId : null;

    if (!documentId) return;

    const docBtn      = document.getElementById("downloadDocBtn");
    const docSpinner  = document.getElementById("downloadDocSpinner");
    const docText     = document.getElementById("downloadDocText");

    // Pre-set the download URLs with the real document ID
    docBtn.href   = `/signing/download/${documentId}`;

    const pollIntervalMs = 2000;
    let pollTimeoutId;

    function pollStatus() {
        fetch(`/signing/status/${documentId}`)
            .then(function (response) {
                if (response.status === 200) {
                    // Document is ready — enable both buttons
                    docSpinner.classList.add("d-none");

                    docText.textContent   = "Download Lease Agreement";

                    docBtn.classList.remove("disabled");
                    docBtn.removeAttribute("aria-disabled");

                    clearTimeout(pollTimeoutId);
                } else {
                    // Not ready yet — keep polling
                    pollTimeoutId = setTimeout(pollStatus, pollIntervalMs);
                }
            })
            .catch(function () {
                // Network error — retry
                pollTimeoutId = setTimeout(pollStatus, pollIntervalMs);
            });
    }

    // Start polling
    pollTimeoutId = setTimeout(pollStatus, pollIntervalMs);
});