using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Archean.App.WebApp;

public class JSInterop
{
    private readonly IJSRuntime _jsRuntime;

    public JSInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task OpenDialogAsync(ElementReference element)
    {
        await _jsRuntime.InvokeVoidAsync("showDialog", element);
    }

    public async Task CloseDialogAsync(ElementReference element)
    {
        await _jsRuntime.InvokeVoidAsync("closeDialog", element);
    }
}
