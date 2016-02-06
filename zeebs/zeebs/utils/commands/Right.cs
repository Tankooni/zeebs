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
	class Right : Command
	{
		public Right()
		{
			CommandName = "right";
		}

		public override Command CreateNewSelf()
		{
			return new Right();
		}
	}
}
