namespace Backsight.Forms;

public static class KeyEventArgsExtensions
{
    extension(KeyEventArgs e)
    {
        public KeySelection KeySelection =>
            new(e.KeyValue, e.Alt, e.Control, e.Shift);
    }
}