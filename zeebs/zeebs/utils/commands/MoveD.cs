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
    public class MoveD : Command
    {
        string movestr;

        public MoveD()
        {
            CommandName = "moved";
        }
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
            {
                FailReasonMessage = "Not part of game";
                return false;
            }
            var match = Regex.Match(commandParams, @"([udlrwasd123456789]+)");
            if (!match.Success)
            {
				FailReasonMessage = "Invalid format. Plese use !moved <uldr wasd 48627913>";
                return false;
            }

            //if (!int.TryParse(match.Groups[1].Value, out dX) || dX < 0 || dX > FP.Width || !int.TryParse(match.Groups[2].Value, out dY) || dY < 0 || dY > FP.Height)
            //{
            //	failMessage = "Values are out of bounds, please enter a value bewtten 0 & " + FP.Width + " for X and bewtween 0 & " + FP.Height + " for Y";
            //	return false;
            //}

            movestr = match.Groups[1].ToString();
            return true;
        }

        public override void Execute()
        {
            var player = Utility.GamePlayers[Args[(int)StdExpMessageValues.UseName]];

            foreach( char c in movestr )
			{
                switch (c)
				{
					case 'u': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, -30); break;
					case 'd': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, 30); break;
					case 'l': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -30, 0); break;
					case 'r': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 30, 0); break;
					case 'w': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, -30); break;
					case 's': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, 30); break;
					case 'a': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -30, 0); break;
					case '8': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, -30); break;
					case '2': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 0, 30); break;
					case '4': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -30, 0); break;
					case '6': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 30, 0); break;
					case '7': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -21, -21); break;
					case '9': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 21, -21); break;
					case '1': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], -21, 21); break;
					case '3': FP.World.BroadcastMessage(MoveDMessage.MoveD, Args[(int)StdExpMessageValues.UseName], 21, 21); break;
					default: break;
                }
            }
        }

        public override Command CreateNewSelf()
        {
            return new MoveD();
        }

        public enum MoveDMessage
        {
            MoveD
        }
    }
}
