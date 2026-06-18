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

// --- Keyboard paging -------------------------------------------------------
// Lets the Left/Right arrow keys turn table pages. Calls back into .NET
// (OnPageKey) with -1 for previous and +1 for next. Key presses are ignored
// while the user is typing in a form control so search / date / dropdown
// inputs keep their normal arrow-key behaviour.
//
// Registrations form a stack so a modal (e.g. EntityListModal) can take over
// paging while it's open and hand it back to the page underneath when it
// closes. Only the topmost registrant receives the keys, and a single shared
// window listener serves them all.
let pageKeyRefs = [];
let pageKeyHandler = null;

export function registerPageKeys(helper) {
    pageKeyRefs.push(helper);
    if (pageKeyHandler) return;

    pageKeyHandler = (e) => {
        if (e.key !== "ArrowLeft" && e.key !== "ArrowRight") return;
        if (e.ctrlKey || e.altKey || e.metaKey) return;

        const el = document.activeElement;
        if (el) {
            const tag = el.tagName;
            if (tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT" || el.isContentEditable) return;
        }

        // Only the topmost (most recently registered) consumer gets the keys.
        const active = pageKeyRefs[pageKeyRefs.length - 1];
        active?.invokeMethodAsync("OnPageKey", e.key === "ArrowRight" ? 1 : -1);
    };
    window.addEventListener("keydown", pageKeyHandler);
}

export function unregisterPageKeys() {
    pageKeyRefs.pop();
    if (pageKeyRefs.length === 0 && pageKeyHandler) {
        window.removeEventListener("keydown", pageKeyHandler);
        pageKeyHandler = null;
    }
}
