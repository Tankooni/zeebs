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
	public class Kick : AdminCommand
	{
		string usernameToBeKicked;

		public Kick()
		{
			CommandName = "kick";
			Helptext = "Kick - ";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			var baseResult = base.CanExecute(args, commandParams, emotes);
			if (!baseResult)
				return baseResult;

			if (string.IsNullOrWhiteSpace(commandParams))
			{
				FailReasonMessage = "No user specified";
				return false;
			}
			usernameToBeKicked = commandParams.Trim().ToLower();
			if (!Utility.SessionPlayers.ContainsKey(usernameToBeKicked))
			{
				FailReasonMessage = "User not in session";
				return false;
			}
			
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(KickMessage.Kick, usernameToBeKicked);
		}

		public override Command CreateNewSelf()
		{
			return new Kick();
		}

		public enum KickMessage
		{
			Kick
		}
		
        public override string GetHelpText() {
            return Helptext;
        }
	}
}
