namespace Backsight.Editor.Map;

public class FetchWindowChangedEventArgs : EventArgs
{
    internal Window NewExtent { get; }
    internal double MapScale { get; }
    
    internal FetchWindowChangedEventArgs(Window newExtent, double mapScale)
    {
        NewExtent = newExtent;
        MapScale = mapScale;
    }
}