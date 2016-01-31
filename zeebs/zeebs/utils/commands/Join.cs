using Indigo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;

namespace zeebs.utils.commands
{
	public class Join : Command
	{
		string emoteName;

		public Join()
		{
			CommandName = "join";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "Already part of the game";
				return false;
			}
			if (Utility.ConnectedPlayers.Count == Utility.MainConfig.MaxPlayers)
			{
				failMessage = "Too many players connected";
				return false;
			}
			var match = Regex.Match(args[(int)StdExpMessageValues.Emotes], @"(\d+):(\d+)-(\d+)");
			if (!match.Success)
			{
				failMessage = "No emote specified. Please use !join <emote>";
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
			FP.World.BroadcastMessage(JoinGameMessage.JoinGame, args[(int)StdExpMessageValues.UseName], emoteName, args[(int)StdExpMessageValues.UserColor]);
		}

		public override Command CreateNewSelf()
		{
			return new Join();
		}

		public enum JoinGameMessage
		{
			JoinGame
		}
	}
}
