using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.metaData
{
	public class AnimatedEntityData
	{
		public string Name;
		public string DefaultAnimation;
		public string ShaderName;
		public Dictionary<string, AnimationData> Animations;
	}
}
