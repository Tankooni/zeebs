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
	public class QuitGame : Command
	{
		string username;

		public QuitGame()
		{
			CommandName = "quit";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);

			username = args[(int)StdExpMessageValues.UseName];
			if (!Utility.SessionPlayers.ContainsKey(username))
			{
				FailReasonMessage = "User not in session";
				return false;
			}
			
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(QuitMessage.Quit, username);
		}

		public override Command CreateNewSelf()
		{
			return new QuitGame();
		}

		public enum QuitMessage
		{
			Quit
		}
	}
}
