namespace Backsight.Editor.Forms;

public static class ISpatialDisplayEx
{
    extension(ISpatialDisplay display)
    {
        public PointF ToPointF(IPosition position)
        {
            var (x, y) = display.GroundToDisplay(position.X, position.Y);
            return new PointF(x, y);
        }
        
        public Point ToPoint(IPosition position)
        {
            var (x, y) = display.GroundToDisplay(position.X, position.Y);
            return new Point((int)x, (int)y);
        }
    }
    
}