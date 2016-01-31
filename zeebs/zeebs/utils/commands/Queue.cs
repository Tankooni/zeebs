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
    public class Queue : Command
    {
        Loop loopCommand = new Loop();

        public Queue()
		{
			CommandName = "queue";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
            return loopCommand.CanExecute(args, out failMessage);
		}

		public override void Execute(string[] args)
		{
            FP.World.BroadcastMessage(LoopMessage.Loop, args, loopCommand.commands, false);
		}

        public override Command CreateNewSelf()
        {
            var queue = new Queue();
            queue.loopCommand = (Loop)this.loopCommand.CreateNewSelf();
            return queue;
        }

		public enum LoopMessage
		{
			Loop
		}
	}
}
