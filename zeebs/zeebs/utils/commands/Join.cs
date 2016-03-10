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
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	public class Join : Command
	{
		string emoteName;
		bool isAvatar;

		public Join()
		{
			CommandName = "join";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Already part of the game";
				return false;
			}
			if (Utility.GamePlayers.Count == Utility.MainConfig.MaxPlayers)
			{
				FailReasonMessage = "Too many players connected";
				return false;
			}

			if (commandParams.ToLower().Contains("avatar"))
			{
				emoteName = args[(int)StdExpMessageValues.UseName];
				isAvatar = true;
			}
			else if (emotes.Count != 0)
			{
				var emote = emotes.First();
				emoteName = commandParams.Substring(emote.StartPos, emote.EndPos - emote.StartPos + 1);
			}
			else
			{
				FailReasonMessage = "No emote specified";
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(JoinGameMessage.JoinGame, Args[(int)StdExpMessageValues.UseName], Args[(int)StdExpMessageValues.DisplayName], emoteName, isAvatar, Args[(int)StdExpMessageValues.UserColor]);
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
