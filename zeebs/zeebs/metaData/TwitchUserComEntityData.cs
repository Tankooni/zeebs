using Indigo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.metaData
{
	public class TwitchUserComEntityData
	{
		public string TwitchUserName;
		public string ComEmoteHead;
		public string ComEntityName;
		public Point ComEntityPosition;
		public Queue<zeebs.entities.ComEntityCommand> CommandQueue;
	}
}
