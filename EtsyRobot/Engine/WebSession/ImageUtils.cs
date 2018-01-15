using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace EtsyRobot.Engine.WebSession
{
	static internal class ImageUtils
	{
		/// <summary>
		/// Converts a byte array to an image.
		/// </summary>
		/// <param name="imageBytes">The byte array.</param>
		/// <returns>An image.</returns>
		static public Image BytesToImage(byte[] imageBytes)
		{
			using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
			{
				ms.Write(imageBytes, 0, imageBytes.Length);
				return Image.FromStream(ms, true);
			}
		}

        static public void DrawRects(Bitmap bitmap, IList<Rectangle> rects, Color color, Boolean fill = false, HatchStyle style = HatchStyle.Min) {
            if (rects.Count == 0) {
                return;
            }
            
            Pen pen = new Pen(color, 1);
            Brush brush = null;
            if (style != HatchStyle.Min) {
                brush = new HatchBrush(
                       style,
                       color,
                       Color.Empty);
            }
            else
            {
                brush = new SolidBrush(color);
            }

            Rectangle[] arr = new Rectangle[rects.Count];
            rects.CopyTo(arr, 0);
            using(var graphics = Graphics.FromImage(bitmap)) 
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                if (!fill)
                {
                    graphics.DrawRectangles(pen, arr);
                }
                else
                {
                    graphics.FillRectangles(brush, arr);
                }
            }
        }
	}
}