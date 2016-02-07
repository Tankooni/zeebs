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
    class Loop : Command
    {

		public static HashSet<string> AllowedLoopCommands = new HashSet<string>{"move", "moved", "attack", "moverandom", "spin", "flip", "color", "change", "up", "down", "left", "right"};

        public Loop()
		{
			CommandName = "loop";
		}

		public override void SetCommandList(List<Command> commands)
		{
			for(int i = commands.Count-1; i >= 0; i--)
				if (!AllowedLoopCommands.Contains(commands[i].CommandName))
					commands.RemoveAt(i);
			base.SetCommandList(commands);
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

		public override bool IsGreedy()
		{
			return true;
		}

		public override void Execute()
		{
			
			FP.World.BroadcastMessage(LoopMessage.Loop, Args[(int)StdExpMessageValues.UseName], Args, Commands);
		}

        public override Command CreateNewSelf()
        {
            return new Loop();
        }

		public enum LoopMessage
		{
			Loop
		}
	}
}
