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
	class Attack : Command
	{
		string emoteName;

		public Attack()
		{
			CommandName = "attack";
			Helptext = "Attack - destroys any zeebs within your radius";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of the game";
				return false;
			}

			//var match = Regex.Match(args[(int)StdExpMessageValues.Emotes], @"(\d+):(\d+)-(\d+)");
			//if (!match.Success)
			//{
			//	failMessage = "No emote specified";
			//	return false;
			//}
			//var startPos = int.Parse(match.Groups[2].Value);
			//var endPos = int.Parse(match.Groups[3].Value);

			//emoteName = args[(int)StdExpMessageValues.Message].Substring(startPos, endPos - startPos + 1);
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(AttackeMessage.Attack, Args[(int)StdExpMessageValues.UseName], Utility.GamePlayers[Args[(int)StdExpMessageValues.UseName]]);
		}

		public override Command CreateNewSelf()
		{
			return new Attack();
		}

		public enum AttackeMessage
		{
			Attack,
			AttackCommand
		}

		public override string GetHelpText() {
			return Helptext;
		}
	}
}
