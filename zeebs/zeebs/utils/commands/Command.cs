using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.utils.commands
{
	public abstract class Command
	{
		List<Command> commands;
		public Command(List<Command> commands)
		{
			this.commands = commands;
		}
		
		public string CommandName;

		public virtual bool CanExecute(string[] args, out string failMessage)
		{
			failMessage = "";
			return false;
		}
		public virtual void Execute(string[] args)
		{

		}

		public abstract Command CreateNewSelf();
	}
}
