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
	public class Leave : Command
	{
		public Leave()
		{
			CommandName = "leave";
		}

		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "";
				return false;
			}
			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(LeaveMessage.Leave, args[(int)StdExpMessageValues.UseName]);
		}

		public enum LeaveMessage
		{
			Leave
		}
	}
}
