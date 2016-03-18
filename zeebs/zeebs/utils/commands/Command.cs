using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	public abstract class Command
	{
		public string CommandName;
		public string FailReasonMessage = "";
		public string[] Args;
		public string CommandParams;
		public List<Command> Commands;
		public List<Emote> Emotes;
		public string helptext;

		public virtual bool IsGreedy()
		{
			return false;
		}

		public virtual void SetCommandList(List<Command> commands)
		{
			Commands = commands;
		}

		public virtual bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			FailReasonMessage = "";
			Args = args;
			CommandParams = commandParams;
			Emotes = emotes;
			return false;
		}
		public virtual void Execute()
		{

		}

		public abstract Command CreateNewSelf();
		
		// Return a help description of the command itself (like man pages!)
		public abstract String GetHelpText();
	}

	
}
