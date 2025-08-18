using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Archean.App.WebApp;

/// <summary>
/// Contains interoperability code used to invoke JavaScript functions.
/// </summary>
public class JSInterop
{
    private readonly IJSRuntime _jsRuntime;

    public JSInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Opens the <c>&lt;Dialog&gt;</c> tag <paramref name="element"/>.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task OpenDialogAsync(ElementReference element)
    {
        await _jsRuntime.InvokeVoidAsync("showDialog", element);
    }

    /// <summary>
    /// Closes the <c>&lt;Dialog&gt;</c> tag <paramref name="element"/>.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task CloseDialogAsync(ElementReference element)
    {
        await _jsRuntime.InvokeVoidAsync("closeDialog", element);
    }
}
