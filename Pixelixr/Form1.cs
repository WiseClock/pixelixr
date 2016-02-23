using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixelixr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fdl = new OpenFileDialog();
            if (fdl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filename = fdl.FileName;
                pictureBox1.Image = Image.FromFile(filename);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            Image i = pictureBox1.Image;
            double width = i.Width;
            double height = i.Height;
            Image result = null;

            double max = (double)maxWidth.Value;

            /*
            if (width > max && height > max)
            {
                if (width > max)
                {
                    height *= max / width;
                    width = max;
                }
                else
                {
                    width *= max / height;
                    height = max;
                }
                Bitmap b = ResizeImage(i, (int)width, (int)(height / 1.82));
                result = b;
            }
            else
                result = (Image)i.Clone();
             */

            if (width > max)
            {
                height *= max / width;
                width = max;
            }
            result = ResizeImage(i, (int)width, (int)(height / 1.82));

            pictureBox2.Image = result;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
                return;
            Thread t = new Thread(a);
            t.Start();
        }

        private void a()
        {
            this.Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text = "";
            });
            Bitmap img = (Bitmap)pictureBox2.Image;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Color c = img.GetPixel(j, i);
                    if (c.A == 0)
                        sb.Append(" ");
                    else
                        sb.Append("[[g;#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") + ";]#]");
                }
                sb.Append(Regex.Unescape(txtSeparator.Text));
            }

            this.Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text = sb.ToString();
                richTextBox1.Focus();
                richTextBox1.SelectAll();
            });
        }
    }
}
