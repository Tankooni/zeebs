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
		public int Frames { set; get; }
		public int FPS { set; get; }
		public Point HeadPosition { set; get; }
		public int HeadWidth { set; get; }
		public Point Origin { set; get; }
	}
}
