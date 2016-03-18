using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	class HypeButton : Command
	{
		public HypeButton()
		{
			CommandName = "hypebutton";
			helptext = "HypeButton - It's lit";

		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			return true;
		}

		public override void Execute()
		{

		}

		public override Command CreateNewSelf()
		{
			return new HypeButton();
		}
		
		public override string GetHelpText() {
			return helptext;
		}
	}
}
