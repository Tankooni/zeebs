using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;


namespace zeebs.utils.commands
{
	class Down : Command
	{
		public Down()
		{
			CommandName = "down";
		}

		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "Not part of game";
				return false;
			}

			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
		}

		public override Command CreateNewSelf()
		{
			return new Down();
		}
	}
}
