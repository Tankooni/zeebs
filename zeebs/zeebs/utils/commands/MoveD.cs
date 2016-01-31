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
    public class MoveD : Command
    {
        string movestr;

        public MoveD()
        {
            CommandName = "moved";
        }
        public override bool CanExecute(string[] args, out string failMessage)
        {
            if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
            {
                failMessage = "Not part of game";
                return false;
            }
            var match = Regex.Match(args[(int)StdExpMessageValues.Message], @"\!moved\s+([udlr]+)");
            if (!match.Success)
            {
                failMessage = "Invalid format. Plese use !move <x_integer> <y_integer>";
                return false;
            }

            //if (!int.TryParse(match.Groups[1].Value, out dX) || dX < 0 || dX > FP.Width || !int.TryParse(match.Groups[2].Value, out dY) || dY < 0 || dY > FP.Height)
            //{
            //	failMessage = "Values are out of bounds, please enter a value bewtten 0 & " + FP.Width + " for X and bewtween 0 & " + FP.Height + " for Y";
            //	return false;
            //}

            movestr = match.Groups[1].ToString();

            failMessage = "";
            return true;
        }

        public override void Execute(string[] args)
        {
            var player = Utility.ConnectedPlayers[args[(int)StdExpMessageValues.UseName]];

            foreach( char c in movestr ) {
                switch (c) {
                    case 'u': FP.World.BroadcastMessage(MoveDMessage.MoveD, args[(int)StdExpMessageValues.UseName], 0, -30); break;
                    case 'd': FP.World.BroadcastMessage(MoveDMessage.MoveD, args[(int)StdExpMessageValues.UseName], 0, 30); break;
                    case 'l': FP.World.BroadcastMessage(MoveDMessage.MoveD, args[(int)StdExpMessageValues.UseName], -30, 0); break;
                    case 'r': FP.World.BroadcastMessage(MoveDMessage.MoveD, args[(int)StdExpMessageValues.UseName], 30, 0); break;
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
