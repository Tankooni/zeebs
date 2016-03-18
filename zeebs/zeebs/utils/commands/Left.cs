﻿using Indigo;
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
	class Left : Command
	{
		public Left()
		{
			CommandName = "left";
			helptext = "Left - ";

		}

		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of game";
				return false;
			}
			
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(MoveD.MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -30, 0);
		}

		public override Command CreateNewSelf()
		{
			return new Left();
		}
		
		public override string GetHelpText() {
			return helptext;
		}
	}
}
