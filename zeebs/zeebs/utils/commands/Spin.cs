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
    class Spin : Command
    {
        public Spin()
		{
			CommandName = "spin";
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
			FP.World.BroadcastMessage(SpinMessage.Spin, args[(int)StdExpMessageValues.UseName]);
		}

        public override Command CreateNewSelf()
        {
            return new Spin();
        }

        public enum SpinMessage
        {
            Spin
        }
    }
}
