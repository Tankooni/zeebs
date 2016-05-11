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
	public abstract class AdminCommand : Command
	{
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (Utility.MainConfig.AdminUser.ToLower() != args[(int)StdExpMessageValues.UseName])
			{
				FailReasonMessage = "Not the admin user";
				return false;
			}
			return true;
		}
	}
}
