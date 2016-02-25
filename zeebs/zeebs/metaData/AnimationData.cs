using Indigo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.metaData
{
	public class AnimationData
	{
		public string Name { set; get; }
		public int OriginX;
		public int OriginY;
		public int FrameWidth;
		public int FrameHeight;
		public int FPS { set; get; }
		public int HeadPositionX;
		public int HeadPositionY;
		public int HeadWidth { set; get; }
		public int[] Frames { set; get; }
	}
}
