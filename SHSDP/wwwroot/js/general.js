import Bowser from '../lib/bowser/bowser.min.js';

export let blazorLoaded = false;

window.loadBlazor = () =>
{
    window.blazorLoaded = true;
};

function getHeightInPx(elem)
{
    return document.getElementById(elem).offsetHeight + "px"
}

function setHeightInPx(elem, height)
{
    document.getElementById(elem).style.height = height;
}

window.dispatchBlazorNavigation = (newUrl) =>
{
    const event = new CustomEvent("blazor:navigation", 
    {
        detail: { url: newUrl }
    });
    window.dispatchEvent(event);
}

document.addEventListener("DOMContentLoaded", () =>
{
    const url = new URL(window.location.href);
    const params = url.searchParams;

    if (params.get("r") === "true") // Force a hard reload
    {
        console.log("Hard reload triggered due to 'r=true' in URL");
        // Remove the "r" param
        params.delete("r");

        // Reconstruct the URL without "r=true"
        url.search = params.toString();

        // Trigger a full reload without "r=true" in the URL
        window.location.replace(url.toString());
    }
});

window.getOperatingSystem = () =>
{
    try
    {
        // Use Bowser.js for more accurate OS detection
        const parser = Bowser.getParser(window.navigator.userAgent);
        const osName = parser.getOSName(true); // true = lowercase
        
        // Map Bowser OS names to our simple categories
        if (osName.includes('windows'))
        {
            return 'windows';
        }
        else if (osName.includes('macos') || osName.includes('mac os'))
        {
            return 'macos';
        }
        else if (osName.includes('linux'))
        {
            return 'linux';
        }
        
        // If we can't determine, return unknown
        return 'unknown';
    }
    catch (error)
    {
        console.error('Error detecting OS with Bowser:', error);
        return 'unknown';
    }
};
