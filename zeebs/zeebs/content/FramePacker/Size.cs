using System;

namespace FramePacker
{
	public struct Size
	{
		public Size(int c, int r)
		{
			Cols = c;
			Rows = r;
		}
		
		public static Size GetDimensions(int frameWidth, int frameHeight, int frameCount)
		{
			var columns = (int) Math.Round(Math.Sqrt(frameCount));
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
