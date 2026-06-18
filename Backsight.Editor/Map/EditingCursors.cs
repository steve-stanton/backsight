using Avalonia;
using Avalonia.Input;
using Backsight.Editor.Properties;

namespace Backsight.Editor.Map;

static class EditingCursors
{
    internal static Cursor AttachPointCursor =>
        CreateCursor(Resources.AttachPointCursor, nameof(AttachPointCursor), 16, 15);
    internal static Cursor DiagonalCursor =>
        CreateCursor(Resources.DiagonalCursor, nameof(DiagonalCursor), 16, 16);
    internal static Cursor GrayReverseArrowCursor =>
        CreateCursor(Resources.GrayReverseArrowCursor, nameof(GrayReverseArrowCursor), 13, 23);
    internal static Cursor GrayWandCursor =>
        CreateCursor(Resources.GrayWandCursor, nameof(GrayWandCursor), 6, 6);
    internal static Cursor HollowSquareCursor =>
        CreateCursor(Resources.HollowSquareCursor, nameof(HollowSquareCursor), 16, 16);
    internal static Cursor MagnifyingGlassCursor =>
        CreateCursor(Resources.MagnifyingGlass, nameof(MagnifyingGlassCursor), 10, 9);
    internal static Cursor NewCenterCursor =>
        CreateCursor(Resources.NewCenterCursor, nameof(NewCenterCursor), 16, 15);
    internal static Cursor PanCursor =>
        CreateCursor(Resources.ClosedHand, nameof(PanCursor), 0, 0);
        //CreateCursor(Resources.MovingCarCursor, nameof(PanCursor), 11, 13);
    internal static Cursor PenCursor =>
        CreateCursor(Resources.PenCursor, nameof(PenCursor), 0, 31);
    internal static Cursor Point1Cursor =>
        CreateCursor(Resources.Point1Cursor, nameof(Point1Cursor), 6, 16);
    internal static Cursor Point2Cursor =>
        CreateCursor(Resources.Point2Cursor, nameof(Point2Cursor), 6, 16);
    internal static Cursor PolygonSubdivisionCursor =>
        CreateCursor(Resources.PolygonSubdivisionCursor, nameof(PolygonSubdivisionCursor), 16, 15);
    internal static Cursor ReverseArrowCursor => 
        CreateCursor(Resources.ReverseArrowCursor, nameof(ReverseArrowCursor), 13, 23);
    internal static Cursor StartPanCursor =>
        //CreateCursor(Resources.CarCursor, nameof(StartPanCursor), 14, 12);
        CreateCursor(Resources.OpenHand, nameof(StartPanCursor), 0, 0);
    internal static Cursor WandCursor =>
        CreateCursor(Resources.WandCursor, nameof(WandCursor), 7, 5);

    private static Cursor CreateCursor(byte[] cursorData, string tag, int x, int y)
    {
        using var s = new MemoryStream(cursorData);
        var bitmap = new Avalonia.Media.Imaging.Bitmap(s);
        Cursor result = new Cursor(bitmap, new PixelPoint(x,y));
        //result.Tag = tag;
        return result;
    }
}