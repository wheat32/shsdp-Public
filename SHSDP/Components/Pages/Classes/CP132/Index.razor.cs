using System.Text.Json;
using Microsoft.AspNetCore.Components;
using SHSDP.Code.Bases;

namespace SHSDP.Components.Pages.Classes.CP132;

public partial class Index : ClassBase
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadConfig("CP132");
        
        // Redirect if it's disabled
        RedirectIfDisabled();
    }
}