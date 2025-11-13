using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Interop;

public class PositionService
{
    private readonly IJSRuntime JsRuntime;

    public PositionService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task<Position> PositionAtWithinViewportAsync(
        ElementReference element,
        double cursorX,
        double cursorY
    )
    {
        var rect = await GetBoundingBoxAsync(element);

        const double offsetX = 10; // distance from cursor
        const double offsetY = 10;
        const double margin = 8; // min distance from viewport edges
        const double tolerance = 2; // accounts for subpixel differences

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

        return new Position()
        {
            X = x,
            Y = y
        };
    }

    public async Task<Position> PositionNextToAsync(
        ElementReference child,
        ElementReference target
    )
    {
        var triggerRect = await GetBoundingBoxAsync(child);
        var submenuRect = await GetBoundingBoxAsync(target);

        const double margin = 8; // distance from viewport edges
        const double offsetX = 4; // small gap between parent and submenu
        const double tolerance = 2;

        var viewportSize = await GetViewportSizeAsync();

        // Default: submenu opens to the right of the trigger
        var x = triggerRect.Right + offsetX;
        var y = triggerRect.Top;

        // Adjust if submenu overflows to the right — open to the left instead
        if (x + submenuRect.Width + margin > viewportSize.Width - tolerance)
        {
            x = triggerRect.Left - submenuRect.Width - offsetX;
        }

        // Adjust if submenu overflows to the bottom
        if (y + submenuRect.Height + margin > viewportSize.Height - tolerance)
        {
            y = Math.Max(margin, viewportSize.Height - submenuRect.Height - margin);
        }

        // Clamp within viewport
        x = Math.Min(Math.Max(x, margin), viewportSize.Width - submenuRect.Width - margin);
        y = Math.Min(Math.Max(y, margin), viewportSize.Height - submenuRect.Height - margin);

        return new Position()
        {
            X = x,
            Y = y
        };
    }

    public async Task<Position> PositionAroundAsync(
        ElementReference child,
        ElementReference target,
        PositionPreference preference
    )
    {
        var triggerRect = await GetBoundingBoxAsync(child);
        var submenuRect = await GetBoundingBoxAsync(target);

        const double margin = 8; // distance from viewport edges
        const double offset = 4; // small gap between parent and submenu
        const double tolerance = 2;

        var viewportSize = await GetViewportSizeAsync();

        Position position;

        switch (preference)
        {
            case PositionPreference.Right:
                position = PositionRight(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);
                
                // Fallback to left if clipping
                if (position.X + submenuRect.Width + margin > viewportSize.Width - tolerance)
                    position = PositionLeft(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);

                break;

            case PositionPreference.Left:
                position = PositionLeft(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);
                
                // Fallback to right if clipping
                if (position.X < margin)
                    position = PositionRight(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);

                break;

            case PositionPreference.Bottom:
                position = PositionBottom(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);
                
                // Fallback to top if clipping
                if (position.Y + submenuRect.Height + margin > viewportSize.Height - tolerance)
                    position = PositionTop(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);

                break;

            case PositionPreference.Top:
                position = PositionTop(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);
                
                // Fallback to bottom if clipping
                if (position.Y < margin)
                    position = PositionBottom(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);

                break;

            default:
                position = PositionRight(triggerRect, submenuRect, viewportSize, offset, margin, tolerance);
                break;
        }

        // Clamp within viewport
        position.X = Math.Min(Math.Max(position.X, margin), viewportSize.Width - submenuRect.Width - margin);
        position.Y = Math.Min(Math.Max(position.Y, margin), viewportSize.Height - submenuRect.Height - margin);

        return position;
    }

    private static Position PositionRight(
        DomRect trigger,
        DomRect submenu,
        ViewportSize viewport,
        double offset,
        double margin,
        double tolerance
    )
    {
        var x = trigger.Right + offset;
        var y = trigger.Top;

        // Adjust vertical position if overflow
        if (y + submenu.Height + margin > viewport.Height - tolerance)
            y = Math.Max(margin, viewport.Height - submenu.Height - margin);

        return new Position(x, y);
    }

    private static Position PositionLeft(
        DomRect trigger,
        DomRect submenu,
        ViewportSize viewport,
        double offset,
        double margin,
        double tolerance
    )
    {
        var x = trigger.Left - submenu.Width - offset;
        var y = trigger.Top;

        // Adjust vertical position if overflow
        if (y + submenu.Height + margin > viewport.Height - tolerance)
            y = Math.Max(margin, viewport.Height - submenu.Height - margin);

        return new Position(x, y);
    }

    private static Position PositionBottom(
        DomRect trigger,
        DomRect submenu,
        ViewportSize viewport,
        double offset,
        double margin,
        double tolerance
    )
    {
        var x = trigger.Left;
        var y = trigger.Bottom + offset;

        // Adjust horizontal position if overflow
        if (x + submenu.Width + margin > viewport.Width - tolerance)
            x = Math.Max(margin, viewport.Width - submenu.Width - margin);

        return new Position(x, y);
    }

    private static Position PositionTop(
        DomRect trigger,
        DomRect submenu,
        ViewportSize viewport,
        double offset,
        double margin,
        double tolerance
    )
    {
        var x = trigger.Left;
        var y = trigger.Top - submenu.Height - offset;

        // Adjust horizontal position if overflow
        if (x + submenu.Width + margin > viewport.Width - tolerance)
            x = Math.Max(margin, viewport.Width - submenu.Width - margin);

        return new Position(x, y);
    }

    private async Task<DomRect> GetBoundingBoxAsync(ElementReference element)
    {
        return await JsRuntime.InvokeAsync<DomRect>("testy.getBoundingBox", element);
    }

    private async Task<ViewportSize> GetViewportSizeAsync()
    {
        return await JsRuntime.InvokeAsync<ViewportSize>("testy.getViewport");
    }

    public struct Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}