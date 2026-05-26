// BoldSign appends ?documentId=xxx to the redirect URL after signing.
// Read it and forward it to the parent window in the shape signdocument.js expects.
var documentId = new URLSearchParams(window.location.search).get("documentId");
parent.postMessage({ action: "onDocumentSigned", documentId: documentId }, "*");