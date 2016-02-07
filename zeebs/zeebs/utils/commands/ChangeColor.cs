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
	class ChangeColor : Command
	{
		public string color;

		public ChangeColor()
		{
			CommandName = "color";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of game";
				return false;
			}

			var match = Regex.Match(commandParams, "#?([A-Fa-f0-9]{6}|random)");
			if(!match.Success)
			{
				FailReasonMessage = "Color in wrong format. Use Hex RRGGBB format or the word random";
				return false;
			}
			if(match.Groups[1].Value == "random")
				color = String.Format("{0:X6}", FP.Random.Int(16777216));
			else
				color = match.Groups[1].Value;

			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(ChangeColorMessage.ChangeColor, Args[(int)StdExpMessageValues.UseName], color);
		}

		public override Command CreateNewSelf()
		{
			return new ChangeColor();
		}

		public enum ChangeColorMessage
		{
			ChangeColor
		}
	}
}
