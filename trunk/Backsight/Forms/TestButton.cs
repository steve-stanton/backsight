using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Backsight.Forms
{
    public partial class TestButton : Button
    {
        Bitmap m_GrayImage;

        public TestButton()
        {
            InitializeComponent();

            m_GrayImage = null;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.Enabled)
                base.OnPaint(pe);
            else
            {
                if (m_GrayImage==null)
                {
                    //Bitmap bm = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                    //Bitmap bm = new Bitmap(Width, Height);
                    //DrawToBitmap(bm, new Rectangle(0, 0, bm.Width, bm.Height));
                    Bitmap bm = new Bitmap(BackgroundImage);
                    m_GrayImage = ConvertToGrayscale(bm);
                }

                pe.Graphics.DrawImage(m_GrayImage, 0, 0, Width, Height);
            }
        }

        // From http://www.bobpowell.net/grayscale.htm
        // This is the simple method, perhaps not the slickest
        public Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap bm = new Bitmap(source.Width, source.Height);

            for (int y=0; y<bm.Height; y+=2)
            {
                for (int x=0; x<bm.Width; x+=2)
                {
                    //bm.SetPixel(x, y, Color.Black);
                    Color c = source.GetPixel(x, y);
                    int luma = (int)(c.R*0.3 + c.G*0.59+ c.B*0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }

            return bm;
        }
    }
}
