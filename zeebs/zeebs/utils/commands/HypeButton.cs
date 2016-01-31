using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.utils.commands
{
	class HypeButton : Command
	{
		public HypeButton()
		{
			CommandName = "hypebutton";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			failMessage = "";
			return true;
		}

		public override Command CreateNewSelf()
		{
			return new HypeButton();
		}

		public override void Execute(string[] args)
		{
			
		}
	}
}
