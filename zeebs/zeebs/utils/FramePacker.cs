using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankooni
{
	/// <summary>
	/// Based on work by Jacob Albano
	/// </summary>
	public static class FramePacker
	{
		public static void PackListOfImagesToMemStream(Bitmap[] imagesToPack, MemoryStream stream, int frameWidth, int frameHeight)
		{
			PackListOfImages(imagesToPack, frameWidth, frameHeight).Save(stream, ImageFormat.Png);
		}
		public static Bitmap PackListOfImages(Bitmap[] imagesToPack, int frameWidth, int frameHeight)
		{
			//int frameWidth = imagesToPack[0].Width;
			//int frameHeight = imagesToPack[0].Height;

			var size = Size.GetDimensions(frameWidth, frameHeight, imagesToPack.Length);
			var sprite = new Bitmap(frameWidth * size.Cols, frameHeight * size.Rows);

			using (var g = Graphics.FromImage(sprite))
			{
				g.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, sprite.Width, sprite.Height);

				int x = 0, y = 0;
				foreach (var bmp in imagesToPack)
				{
					var brush = new TextureBrush(bmp);
					g.FillRectangle(brush, x * frameWidth, y * frameHeight, frameWidth, frameHeight);

					if (++x >= size.Cols)
					{
						x = 0;
						y++;
					}
				}
			}
			return sprite;
		}

		private struct Size
		{
			public Size(int c, int r)
			{
				Cols = c;
				Rows = r;
			}

			public static Size GetDimensions(int frameWidth, int frameHeight, int frameCount)
			{
				var columns = (int)Math.Round(Math.Sqrt(frameCount));
				var rows = columns;

				if (columns * rows < frameCount)
				{
					if (rows * frameHeight <= columns * frameWidth) rows++;
					else columns++;
				}

				return new Size(columns, rows);
			}

			public int Cols, Rows;
		}
	}
}
