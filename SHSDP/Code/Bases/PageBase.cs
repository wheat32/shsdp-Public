using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace SHSDP.Code.Bases;

public class PageBase : ComponentBase, IDisposable
{
    [Inject]
    protected NavigationManager? NavigationManager { get; set; }
    [Inject]
    protected IJSRuntime? JSRuntime { get; set; }

    private string? _lastUri;

    protected override void OnInitialized()
    {
        _lastUri = NavigationManager?.Uri;
        NavigationManager!.LocationChanged += OnLocationChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime!.InvokeVoidAsync("loadBlazor");
        }
    }
    
    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Avoid duplicate calls
        if (e.Location != _lastUri)
        {
            _lastUri = e.Location;
            await JSRuntime!.InvokeVoidAsync("dispatchBlazorNavigation", e.Location);
        }
    }

    public void Dispose()
    {
        NavigationManager!.LocationChanged -= OnLocationChanged;
    }
}