export function onLoad()
{
    expandAccordionBasedOnOS();
}

export function onUpdate()
{
    // console.log('Updated');
}

export function onDispose()
{
    // console.log('Disposed');
}

function expandAccordionBasedOnOS()
{
    const os = window.getOperatingSystem();
    
    // Show Linux warning if on Linux
    const linuxNotice = document.getElementById('linuxNotice');
    if (linuxNotice && os === 'linux')
    {
        linuxNotice.style.display = 'block';
    }
    
    // Collapse all accordion items first
    const allCollapses = document.querySelectorAll('#teamsAccordion .accordion-collapse');
    allCollapses.forEach(collapse =>
    {
        collapse.classList.remove('show');
    });
    
    // Update all buttons to collapsed state
    const allButtons = document.querySelectorAll('#teamsAccordion .accordion-button');
    allButtons.forEach(button =>
    {
        button.classList.add('collapsed');
        button.setAttribute('aria-expanded', 'false');
    });
    
    // Expand the appropriate section based on OS
    let targetCollapseId = null;
    
    switch(os)
    {
        case 'windows':
            targetCollapseId = 'collapseWindows';
            break;
        case 'macos':
            targetCollapseId = 'collapseMac';
            break;
        case 'linux':
            targetCollapseId = 'collapseLinux';
            break;
        default:
            // Don't expand any if OS is unknown
            return;
    }
    
    // Expand the target section
    if (targetCollapseId)
    {
        const targetCollapse = document.getElementById(targetCollapseId);
        const targetButton = document.querySelector(`[data-bs-target="#${targetCollapseId}"]`);
        
        if (targetCollapse && targetButton)
        {
            targetCollapse.classList.add('show');
            targetButton.classList.remove('collapsed');
            targetButton.setAttribute('aria-expanded', 'true');
        }
    }
}

