using Indigo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	class SaveScores : Command
	{
		public SaveScores()
		{
			CommandName = "savescores";
			helptext = "SaveScore - ";

		}

		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (args[(int)StdExpMessageValues.UseName] != Utility.MainConfig.AdminUser.ToLower())
			{
				FailReasonMessage = "Nope";
				return false;
			};
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(SaveScoresMessage.SaveScores);
		}

		public override Command CreateNewSelf()
		{
			return new SaveScores();
		}

		public enum SaveScoresMessage
		{
			SaveScores
		}
		
		public override string GetHelpText() {
			return helptext;
		}
	}
}