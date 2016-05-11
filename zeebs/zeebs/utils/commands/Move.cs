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
	public class Move : Command
	{
		int dX;
		int dY;

		public Move()
		{
			CommandName = "move";
   		Helptext = "Move - ";

		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of game";
				return false;
			}
			var match = Regex.Match(commandParams, @"(\d+)\s+(\d+)");
			if (!match.Success)
			{
				FailReasonMessage = "Invalid format. Plese use !move <x_integer> <y_integer>";
				return false;
			}

			//if (!int.TryParse(match.Groups[1].Value, out dX) || dX < 0 || dX > FP.Width || !int.TryParse(match.Groups[2].Value, out dY) || dY < 0 || dY > FP.Height)
			//{
			//	failMessage = "Values are out of bounds, please enter a value bewtten 0 & " + FP.Width + " for X and bewtween 0 & " + FP.Height + " for Y";
			//	return false;
			//}

			if (!int.TryParse(match.Groups[1].Value, out dX) || !int.TryParse(match.Groups[2].Value, out dY))
			{
				FailReasonMessage = "One of the values is higher than int.max. Stahhhhhhpp";
				return false;
			}

			if (FP.World.CollidePoint("ClickMap", dX, dY) != null)
			{
				FailReasonMessage = "Blocked by movemap";
				return false;
			}

			FailReasonMessage = "";
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(MoveMessage.Move, Args[(int)StdExpMessageValues.UseName], dX, dY);
		}

		public override Command CreateNewSelf()
		{
			return new Move();
		}

		public enum MoveMessage
		{
			Move
		}
		
		public override string GetHelpText() {
			return Helptext;
		}
	}
}
