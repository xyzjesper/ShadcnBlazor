using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Test.UI.Components;

public class PositionService
{
    private readonly IJSRuntime JsRuntime;

    public PositionService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task<(double, double)> PositionAtWithinViewportAsync(ElementReference element, double cursorX,
        double cursorY)
    {
        await RequestAnimationFrameAsync();

        var rect = await GetBoundingBoxAsync(element);

        double offsetX = 10; // distance from cursor
        double offsetY = 10;
        double margin = 8; // min distance from viewport edges
        double tolerance = 2; // accounts for subpixel differences

        var x = cursorX + offsetX;
        var y = cursorY + offsetY;

        // Viewport size
        var viewportSize = await GetViewportSizeAsync();

        // If dropdown overflowed to the right, move left
        if (x + rect.Width + margin > viewportSize.Width - tolerance)
        {
            x = Math.Max(margin, cursorX - rect.Width - offsetX);
        }

        // If dropdown overflowed to the bottom, move up
        if (y + rect.Height + margin > viewportSize.Height - tolerance)
        {
            y = Math.Max(margin, cursorY - rect.Height - offsetY);
        }

        // Prevent going off-screen entirely
        x = Math.Min(Math.Max(x, margin), viewportSize.Width - rect.Width - margin);
        y = Math.Min(Math.Max(y, margin), viewportSize.Height - rect.Height - margin);

        return (x, y);
    }

    public async Task<(double, double)> PositionNextToAsync(
        ElementReference child,
        ElementReference target
    )
    {
        var triggerRect = await GetBoundingBoxAsync(child);
        var submenuRect = await GetBoundingBoxAsync(target);

        double margin = 8;  // distance from viewport edges
        double offsetX = 4; // small gap between parent and submenu
        double tolerance = 2;

        var viewportSize = await GetViewportSizeAsync();

        // Default: submenu opens to the right of the trigger
        var x = triggerRect.Right + offsetX;
        var y = triggerRect.Top;

        // Adjust if submenu overflows to the right — open to the left instead
        if (x + submenuRect.Width + margin > viewportSize.Width - tolerance) {
            x = triggerRect.Left - submenuRect.Width - offsetX;
        }

        // Adjust if submenu overflows to the bottom
        if (y + submenuRect.Height + margin > viewportSize.Height - tolerance) {
            y = Math.Max(margin, viewportSize.Height - submenuRect.Height - margin);
        }

        // Clamp within viewport
        x = Math.Min(Math.Max(x, margin), viewportSize.Width - submenuRect.Width - margin);
        y = Math.Min(Math.Max(y, margin), viewportSize.Height - submenuRect.Height - margin);

        return (x, y);
    }

    private async Task RequestAnimationFrameAsync()
    {
        await JsRuntime.InvokeVoidAsync("testy.requestAnimationFrameAsync");
    }

    private async Task<DomRect> GetBoundingBoxAsync(ElementReference element)
    {
        return await JsRuntime.InvokeAsync<DomRect>("testy.getBoundingBox", element);
    }

    private async Task<ViewportSize> GetViewportSizeAsync()
    {
        return await JsRuntime.InvokeAsync<ViewportSize>("testy.getViewport");
    }
}