// Viewport helpers for components that size themselves to the window.
// Loaded as a module via JS.InvokeAsync<IJSObjectReference>("import", "./js/viewport.js").

let dotNetRef = null;
let handler = null;
let debounceTimer = 0;

export function getInnerHeight() {
    return window.innerHeight;
}

// Calls back into .NET (OnViewportResized) on resize, debounced so a drag
// doesn't fire a storm of re-renders.
export function registerResize(helper) {
    dotNetRef = helper;
    handler = () => {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            dotNetRef?.invokeMethodAsync("OnViewportResized", window.innerHeight);
        }, 200);
    };
    window.addEventListener("resize", handler);
}

export function unregisterResize() {
    if (handler) {
        window.removeEventListener("resize", handler);
        handler = null;
    }
    clearTimeout(debounceTimer);
    dotNetRef = null;
}
