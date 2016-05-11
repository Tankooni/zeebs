using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;
using zeebs.utils.commands;
using zeebs.utils.zoopBoot;

namespace zeebs.utils
{
	class Part : Command
	{
		public Part()
		{
			CommandName = "part";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[1]) || args[2] != "PART")
			{
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(Leave.LeaveMessage.Leave, Args[1]);
		}

		public override Command CreateNewSelf()
		{
			return new Part();
		}
	}
}
