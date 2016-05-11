using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	class Cancel : Command
	{
		public Cancel()
		{
			CommandName = "cancel";
   			Helptext = "Cancel - ";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of game";
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(CancelMessage.Cancel, Args[(int)StdExpMessageValues.UseName]);
		}

		public override Command CreateNewSelf()
		{
			return new Cancel();
		}

		public enum CancelMessage
		{
			Cancel
		}

		public override string GetHelpText() {
			return Helptext;
	}
	}
}
