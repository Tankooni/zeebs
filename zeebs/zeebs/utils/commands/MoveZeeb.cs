using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;

namespace zeebs.utils.commands
{
	public class MoveZeeb : Command
	{
		int dX;
		int dY;

		public MoveZeeb()
		{
			CommandName = "movezeeb";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[9]))
			{
				failMessage = "Not part of game";
				return false;
			}
			var match = Regex.Match(args[12], @"(\d+)\s+(\d+)");
			if (!match.Success)
			{
				failMessage = "Invalid format. Plese use !movezeeb <x_integer> <y_integer>";
				return false;
			}
			if((dX = int.Parse(match.Groups[1].Value)) < 0 || dX > FP.Width || (dY = int.Parse(match.Groups[2].Value)) < 0 || dY > FP.Height)
			{
				failMessage = "Values are out of bounds, please enter a value bewtten 0 & " + FP.Width + " for X and bewtween 0 & " + FP.Height + "for Y";
				return false;
			}

			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
			FP.World.BroadcastMessage(MoveZeebMessage.MoveZeeb, args[9], dX, dY);
		}

		public enum MoveZeebMessage
		{
			MoveZeeb
		}
	}
}
