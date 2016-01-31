using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;
using zeebs.utils.commands;

namespace zeebs.utils
{
	class Part : Command
	{
		public Part()
		{
			CommandName = "part";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[1]) || args[2] != "PART")
			{
				failMessage = "";
				return false;
			}
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(Leave.LeaveMessage.Leave, args[1]);
		}
	}
}
