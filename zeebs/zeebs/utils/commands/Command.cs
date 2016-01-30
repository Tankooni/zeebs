using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.utils.commands
{
	public abstract class Command
	{
		public string CommandName;

		public virtual bool CanExecute(string[] args)
		{
			return false;
		}
		public virtual void Execute(string[] args)
		{

		}
	}
}
