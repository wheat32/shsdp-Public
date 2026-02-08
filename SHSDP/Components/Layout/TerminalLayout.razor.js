// TerminalLayout.razor.js

let observer = null;

export function onLoad() 
{
    const term = document.getElementById('terminal-body');
    if (!term) return;

    // Ensure we clean up any previous observer
    if (observer) 
    {
        observer.disconnect();
        observer = null;
    }

    observer = new MutationObserver(() => 
    {
        // Scroll when DOM changes (new lines, edits, etc.)
        term.scrollTop = term.scrollHeight;
    });

    observer.observe(term, 
    {
        childList: true,       // watch for added/removed elements
        subtree: true,         // include all nested nodes
        characterData: true    // detect text changes inside existing nodes
    });

    // Also scroll once immediately to the bottom (on load)
    term.scrollTop = term.scrollHeight;
}

export function onUpdate() 
{
    // Optional: ensure scroll remains correct after re-render
    const term = document.getElementById('terminal-body');
    if (term) 
    {
        term.scrollTop = term.scrollHeight;
    }
}

export function onDispose() 
{
    if (observer) 
    {
        observer.disconnect();
        observer = null;
    }
}
