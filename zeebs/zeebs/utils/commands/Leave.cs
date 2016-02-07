using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	public class Leave : Command
	{
		public Leave()
		{
			CommandName = "leave";
		}

		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of game";
				return false;
			};
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(LeaveMessage.Leave, Args[(int)StdExpMessageValues.UseName]);
		}

		public override Command CreateNewSelf()
		{
			return new Leave();
		}

		public enum LeaveMessage
		{
			Leave
		}
	}
}
