using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace zeebs.utils.commands
{
	public class PartGame : Command
	{
		public PartGame()
		{
			CommandName = "partgame";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			if (String.IsNullOrWhiteSpace(args[2]))
				return;
			var match = Regex.Match(args[2], @"(\d+):(\d+)-(\d+)");
			if (!match.Success)
				return;
			var startPos = int.Parse(match.Groups[2].Value);
			var endPos = int.Parse(match.Groups[3].Value);

			var emoteName = args[12].Substring(startPos, endPos - startPos + 1);
			FP.World.BroadcastMessage(PartGameMessage.PartGame, emoteName);
		}

		public enum PartGameMessage
		{
			PartGame
		}
	}
}
