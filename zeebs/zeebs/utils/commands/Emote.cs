using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using System.Text.RegularExpressions;
using Tankooni.IRC;

namespace zeebs.utils.commands
{
	public class Emote : Command
	{
		public Emote()
		{
			CommandName = "emote";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{

			if (String.IsNullOrWhiteSpace(args[(int)StdExpMessageValues.Emotes]))
				return;
			var match = Regex.Match(args[(int)StdExpMessageValues.Emotes], @"(\d+):(\d+)-(\d+)");
			if (!match.Success)
				return;
			var startPos = int.Parse(match.Groups[2].Value);
			var endPos = int.Parse(match.Groups[3].Value);

			var emoteName = args[(int)StdExpMessageValues.Message].Substring(startPos, endPos - startPos + 1);
			FP.World.BroadcastMessage(EmoteMessage.Emote, emoteName);
		}

		public override Command CreateNewSelf()
		{
			return new Emote();
		}

		public enum EmoteMessage
		{
			Emote
		}
	}
}
