using System.Windows;

namespace LoLMatchup.Services.Settings;

public static class WindowBoundsService
{
    private const double AccessibleWidth = 80;
    private const double AccessibleHeight = 32;

    public static Size ValidateSize(double? width, double? height, Size fallback)
    {
        var maximumWidth = Math.Max(420, SystemParameters.VirtualScreenWidth);
        var maximumHeight = Math.Max(420, SystemParameters.VirtualScreenHeight);
        return new Size(
            IsFinitePositive(width) ? Math.Clamp(width!.Value, 420, maximumWidth) : fallback.Width,
            IsFinitePositive(height) ? Math.Clamp(height!.Value, 420, maximumHeight) : fallback.Height);
    }

    public static Point? ValidatePosition(double? left, double? top, Size size)
    {
        if (!IsFinite(left) || !IsFinite(top))
        {
            return null;
        }

        var saved = new Rect(left!.Value, top!.Value, size.Width, size.Height);
        var virtualDesktop = new Rect(
            SystemParameters.VirtualScreenLeft,
            SystemParameters.VirtualScreenTop,
            SystemParameters.VirtualScreenWidth,
            SystemParameters.VirtualScreenHeight);
        var intersection = Rect.Intersect(saved, virtualDesktop);

        return !intersection.IsEmpty
            && intersection.Width >= AccessibleWidth
            && intersection.Height >= AccessibleHeight
                ? saved.TopLeft
                : null;
    }

    public static Point CenterInPrimaryWorkArea(Size size)
    {
        var workArea = SystemParameters.WorkArea;
        return new Point(
            workArea.Left + Math.Max(0, (workArea.Width - size.Width) / 2),
            workArea.Top + Math.Max(0, (workArea.Height - size.Height) / 2));
    }

    private static bool IsFinitePositive(double? value) =>
        IsFinite(value) && value > 0;

    private static bool IsFinite(double? value) =>
        value.HasValue && double.IsFinite(value.Value);
}
