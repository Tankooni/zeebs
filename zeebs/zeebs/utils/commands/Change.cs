using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;

namespace zeebs.utils.commands
{
	class Change : Command
	{
		string emoteName;

		public Change()
		{
			CommandName = "change";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "Not part of the game";
				return false;
			}
			
			var match = Regex.Match(args[(int)StdExpMessageValues.Emotes], @"(\d+):(\d+)-(\d+)");
			if (!match.Success)
			{
				failMessage = "No emote specified";
				return false;
			}
			var startPos = int.Parse(match.Groups[2].Value);
			var endPos = int.Parse(match.Groups[3].Value);

			emoteName = args[(int)StdExpMessageValues.Message].Substring(startPos, endPos - startPos + 1);
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(ChangeMessage.Change, args[(int)StdExpMessageValues.UseName], emoteName);
		}

		public override Command CreateNewSelf()
		{
			return new Change();
		}

		public enum ChangeMessage
		{
			Change
		}
	}
}
