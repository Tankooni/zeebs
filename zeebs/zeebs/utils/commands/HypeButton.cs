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
		public override bool CanExecute(string[] args)
		{
			return true;
		}

		public override void Execute(string[] args)
		{
			
		}
	}
}
