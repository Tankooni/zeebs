using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glide;
using Indigo;
using Indigo.Core;
using zeebs.utils.commands;
using System.Collections;

namespace zeebs.entities.ComEntities.Commands
{
    class ComEntityLoop : ComEntityCommand
	{
        List<Command> commands;
        string[] args;
        bool shouldLoop = true;
        bool isDone = false;

        public ComEntityLoop(ComEntity comEntity, List<Command> commands, string[] args, bool shouldLoop)
			: base(comEntity)
		{
			this.commands = commands;
            this.args = args;
            this.shouldLoop = shouldLoop;
		}

		public override bool IsDone()
		{
            return isDone;
		}

		public override IEnumerator Update()
		{
			isDone = true;
			if (comEntity.CountInQueue() > 1)
            {
                //we're done here
                yield break;
            }

			foreach ( Command command in commands ) {
                command.Execute(args);
            }
            if (shouldLoop)
            {
                comEntity.QueueCommand(new ComEntityLoop(comEntity, commands, args, shouldLoop));
            }
		}
	}
}
