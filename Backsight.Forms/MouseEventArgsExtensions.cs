namespace Backsight.Forms;

public static class MouseEventArgsExtensions
{
    extension(MouseEventArgs e)
    {
        public MouseButton MouseButton => e.Button switch
        {
            MouseButtons.Left => MouseButton.Left,
            MouseButtons.Middle => MouseButton.Middle,
            MouseButtons.Right => MouseButton.Right,
            _ => MouseButton.None
        };
    }
    
}