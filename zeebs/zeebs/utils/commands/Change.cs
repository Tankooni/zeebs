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
	class Change : Command
	{
		string emoteName;

		public Change()
		{
			CommandName = "change";
			helptext = "Change - ";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of the game";
				return false;
			}
			
			if (emotes.Count == 0)
			{
				FailReasonMessage = "No emote specified";
				return false;
			}
			var emote = emotes.First();

			emoteName = commandParams.Substring(emote.StartPos, emote.EndPos - emote.StartPos + 1);
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(ChangeMessage.Change, Args[(int)StdExpMessageValues.UseName], emoteName);
		}

		public override Command CreateNewSelf()
		{
			return new Change();
		}

		public enum ChangeMessage
		{
			Change
		}

		public override string GetHelpText() {
			return helptext;
		}
	}
}
