﻿using Indigo;
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

		public Join()
		{
			CommandName = "join";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Already part of the game";
				return false;
			}
			if (Utility.ConnectedPlayers.Count == Utility.MainConfig.MaxPlayers)
			{
				FailReasonMessage = "Too many players connected";
				return false;
			}

			if (emotes.Count == 0)
			{
				FailReasonMessage = "No emote specified. Please use !join <emote>";
				return false;
			}
			var emote = emotes.First();

			emoteName = commandParams.Substring(emote.StartPos, emote.EndPos - emote.StartPos + 1);
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(JoinGameMessage.JoinGame, Args[(int)StdExpMessageValues.UseName], emoteName, Args[(int)StdExpMessageValues.UserColor]);
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
