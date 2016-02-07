using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.utils.zoopBoot
{
	public class Emote
	{
		public string TwitchID;
		public int StartPos;
		public int EndPos;

		public Emote(string twitchID, int startPos, int endPos)
		{
			TwitchID = twitchID;
			StartPos = startPos;
			EndPos = endPos;
		}
	}
}
