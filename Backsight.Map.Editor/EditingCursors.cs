using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Backsight.Map.Editor;

internal static class EditingCursors
{
    internal static Cursor StartPanCursor => CreateCursor("OpenHand.png", 0, 0);
    internal static Cursor HandCursor => CreateCursor("HandCursor.png", 7, 5);

    private static Cursor CreateCursor(string assetName, int x, int y)
    {
        var uri = new Uri($"avares://Backsight.Map.Editor/Assets/{assetName}");

        using var stream = AssetLoader.Open(uri);
        using var bitmap = new Bitmap(stream);

        return new Cursor(bitmap, new PixelPoint(x, y));
    }
}