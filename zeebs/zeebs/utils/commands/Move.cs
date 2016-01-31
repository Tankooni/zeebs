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
	public class Move : Command
	{
		int dX;
		int dY;

		public Move()
		{
			CommandName = "move";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				failMessage = "Not part of game";
				return false;
			}
			var match = Regex.Match(args[(int)StdExpMessageValues.Message], @"(\d+)\s+(\d+)");
			if (!match.Success)
			{
				failMessage = "Invalid format. Plese use !move <x_integer> <y_integer>";
				return false;
			}
			
			if (!int.TryParse(match.Groups[1].Value, out dX) || dX < 0 || dX > FP.Width || !int.TryParse(match.Groups[2].Value, out dY) || dY < 0 || dY > FP.Height)
			{
				failMessage = "Values are out of bounds, please enter a value bewtten 0 & " + FP.Width + " for X and bewtween 0 & " + FP.Height + " for Y";
				return false;
			}

			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(MoveMessage.Move, args[(int)StdExpMessageValues.UseName], dX, dY);
		}

		public override Command CreateNewSelf()
		{
			return new Move();
		}

		public enum MoveMessage
		{
			Move
		}
	}
}
