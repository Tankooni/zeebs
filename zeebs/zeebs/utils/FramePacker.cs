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
		public static PackedSpriteMapMeta PackListOfImagesToMemStream(Frame[] imagesToPack, MemoryStream stream, int frameWidth, int frameHeight, FrameDisposal frameDisposal)
		{
			var packedSpriteMap = PackListOfImages(imagesToPack, frameWidth, frameHeight, frameDisposal);
			packedSpriteMap.Image.Save(stream, ImageFormat.Png);
			return packedSpriteMap;
		}
		public static PackedSpriteMapMeta PackListOfImages(Frame[] imagesToPack, int frameWidth, int frameHeight, FrameDisposal frameDisposal)
		{
			var size = Size.GetDimensions(frameWidth, frameHeight, imagesToPack.Length);
			var sprite = new Bitmap(frameWidth * size.Cols, frameHeight * size.Rows);
			var spriteFrame = new Bitmap(frameWidth, frameHeight);

			using (var g = Graphics.FromImage(sprite))
			{
				using (var sg = Graphics.FromImage(spriteFrame))
				{
					g.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, sprite.Width, sprite.Height);
					sg.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, sprite.Width, sprite.Height);

					int x = 0, y = 0;
					for (int i = 0; i < imagesToPack.Count(); i++)
					{
						var bmp = imagesToPack[i];
						var brush = new TextureBrush(bmp.Image, System.Drawing.Drawing2D.WrapMode.Clamp);

						brush.TranslateTransform(bmp.X, bmp.Y);

						if (frameDisposal == FrameDisposal.Composite)
						{
							sg.FillRectangle(brush, 0, 0, bmp.Width + bmp.X, bmp.Height + bmp.Y);
							brush.Dispose();
							brush = new TextureBrush(spriteFrame, System.Drawing.Drawing2D.WrapMode.Clamp);
						}


						brush.TranslateTransform(x * frameWidth, y * frameHeight);
						g.FillRectangle(brush, x * frameWidth, y * frameHeight, bmp.Width + bmp.X, bmp.Height + bmp.Y);

						brush.Dispose();
						if (++x >= size.Cols)
						{
							x = 0;
							y++;
						}
					}
				}
			}
			return new PackedSpriteMapMeta
			{
				Image = sprite,
				FrameHeight = frameHeight,
				FrameWidth = frameWidth,
				TotalFrames = imagesToPack.Length,
				Columns = size.Cols,
				Rows = size.Rows
			};
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

		public class Frame
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;
			public Bitmap Image;
		}

		public class PackedSpriteMapMeta
		{
			[Newtonsoft.Json.JsonIgnore]
			public Bitmap Image;
			public int FrameWidth;
			public int FrameHeight;
			public int TotalFrames;
			public int Columns;
			public int Rows;
			public float FPS;
		}

		public enum FrameDisposal
		{
			Replace,
			Composite
		}
	}
}
