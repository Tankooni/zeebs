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
    class Flip : Command
    {
        public Flip()
        {
            CommandName = "flip";
        }
        public override bool CanExecute(string[] args, out string failMessage)
        {
            if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
            {
                failMessage = "Not part of game";
                return false;
            }

            failMessage = "";
            return true;
        }

        public override void Execute(string[] args)
        {
            FP.World.BroadcastMessage(FlipMessage.Flip, args[(int)StdExpMessageValues.UseName]);
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
