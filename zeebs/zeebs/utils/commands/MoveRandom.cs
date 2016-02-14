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
    public class MoveRandom : Command
    {
		int dX;
		int dY;

		public MoveRandom()
        {
            CommandName = "moverandom";
        }
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
            {
				FailReasonMessage = "Not part of game";
                return false;
            }

			do
			{
				dX = FP.Random.Int(0, FP.Width);
				dY = FP.Random.Int(0, FP.Height);
			} while (FP.World.CollidePoint("ClickMap", dX, dY) == null);

            return true;
        }

        public override void Execute()
        {
            FP.World.BroadcastMessage(Move.MoveMessage.Move, Args[(int)StdExpMessageValues.UseName], dX, dY);
        }

        public override Command CreateNewSelf()
        {
            return new MoveRandom();
        }
    }
}
