using Indigo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;

namespace zeebs.utils.commands
{
	public class JoinGame : Command
	{
		string emoteName;

		public JoinGame()
		{
			CommandName = "joingame";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (Utility.ConnectedPlayers.Count == Utility.MainConfig.MaxPlayers)
			{
				failMessage = "Too many players connected";
				return false;
			}
			if (Utility.ConnectedPlayers.ContainsKey(args[9]))
			{
				failMessage = "Already part of the game";
				return false;
			}
			var match = Regex.Match(args[2], @"(\d+):(\d+)-(\d+)");
			if (!match.Success)
			{
				failMessage = "No emote specified";
				return false;
			}
			var startPos = int.Parse(match.Groups[2].Value);
			var endPos = int.Parse(match.Groups[3].Value);

			emoteName = args[12].Substring(startPos, endPos - startPos + 1);
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(JoinGameMessage.JoinGame, args[9], emoteName);
		}

		public enum JoinGameMessage
		{
			JoinGame
		}
	}
}
