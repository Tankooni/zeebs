using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glide;
using Indigo;
using Indigo.Core;
using zeebs.utils.commands;

namespace zeebs.entities.ComEntities.Commands
{
    class ComEntityLoop : ComEntityCommand
	{
        List<Command> commands;
        string[] args;
        bool isDone = false;

        public ComEntityLoop(ComEntity comEntity, List<Command> commands, string[] args)
			: base(comEntity)
		{
			this.commands = commands;
            this.args = args;
		}

		public override bool IsDone()
		{
            return isDone;
		}

		public override void Update()
		{
            if (comEntity.CountInQueue() > 1)
            {
                //we're done here
                return;
            }

			foreach ( Command command in commands ) {
                command.Execute(args);
            }
            comEntity.QueueCommand(new ComEntityLoop(comEntity, commands, args));
            isDone = true;
		}
	}
}
