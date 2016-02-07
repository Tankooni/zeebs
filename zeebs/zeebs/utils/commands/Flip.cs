using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
    class Flip : Command
    {
        public Flip()
        {
            CommandName = "flip";
        }
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
            {
				FailReasonMessage = "Not part of game";
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            FP.World.BroadcastMessage(FlipMessage.Flip, Args[(int)StdExpMessageValues.UseName]);
        }

        public override Command CreateNewSelf()
        {
            return new Flip();
        }

        public enum FlipMessage
        {
            Flip
        }
    }
}
