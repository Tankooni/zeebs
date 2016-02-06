using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;

namespace zeebs.utils.commands
{
	abstract class MoveDirectionsBase : Command
	{
		public List<Command> commands = new List<Command>();

		public MoveDirectionsBase()
		{

		}

		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "Not part of game";
				return false;
			}

			foreach(var match in TwitchInterface.regExers[RegexTypes.AllCommands].Matches(args[(int)StdExpMessageValues.Message]))
			{

			}

			failMessage = "";
			return true;
		}
	}
}
