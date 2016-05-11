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
	class Change : Command
	{
		string emoteName;
		bool isAvatar = false;

		public Change()
		{
			CommandName = "change";
			Helptext = "Change - ";
		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Not part of the game";
				return false;
			}

			var match = Regex.Match(commandParams, @"([\w\d]+)(\s+)?");
			if (!match.Success)
			{
				FailReasonMessage = "No emote specified";
				return false;
			}

			if (match.Groups[1].Value.ToLower() == "avatar")
			{
				emoteName = args[(int)StdExpMessageValues.UseName];
				isAvatar = true;
			}
			else if (emotes.Count != 0)
			{
				var emote = emotes.First();
				emoteName = commandParams.Substring(emote.StartPos, emote.EndPos - emote.StartPos + 1);
			}
			else if (Utility.Twitchy.SpecialEmotes.Contains(match.Groups[1].Value))
			{
				emoteName = match.Groups[1].Value;
			}
			else
			{
				FailReasonMessage = "No emote specified";
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			FP.World.BroadcastMessage(ChangeMessage.Change, Args[(int)StdExpMessageValues.UseName], emoteName, isAvatar);
		}

		public override Command CreateNewSelf()
		{
			return new Change();
		}

		public enum ChangeMessage
		{
			Change,
			ChangeAvatar
		}
		public override string GetHelpText() {
			return Helptext;
		}
	}
}
