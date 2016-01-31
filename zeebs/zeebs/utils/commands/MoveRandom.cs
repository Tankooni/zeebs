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
    public class MoveRandom : Command
    {
        public MoveRandom()
        {
            CommandName = "moverandom";
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
            int dX = FP.Random.Int(0, FP.Width);
            int dY = FP.Random.Int(0, FP.Height);
            FP.World.BroadcastMessage(Move.MoveMessage.Move, args[(int)StdExpMessageValues.UseName], dX, dY);
        }

        public override Command CreateNewSelf()
        {
            return new MoveRandom();
        }
    }
}
