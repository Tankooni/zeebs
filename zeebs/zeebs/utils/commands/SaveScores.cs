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
	class SaveScores : AdminCommand
	{
		public SaveScores()
		{
			CommandName = "savescores";
			Helptext = "SaveScore - ";

		}

		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			return base.CanExecute(args, commandParams, emotes);
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
			return Helptext;
		}
	}
}