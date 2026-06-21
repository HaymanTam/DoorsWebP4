// Triggers a browser "Save as" for bytes produced server-side. The data arrives as a Blazor
// DotNetStreamReference (so we avoid base64-bloating the interop boundary): we read it into an
// ArrayBuffer, wrap it in a Blob, and click a temporary object-URL anchor.
export async function downloadFileFromStream(fileName, contentStreamReference, contentType) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: contentType || 'application/octet-stream' });
    const url = URL.createObjectURL(blob);

    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName || 'download';
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();

    URL.revokeObjectURL(url);
}
