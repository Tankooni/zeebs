using Indigo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tankooni;
using Tankooni.IRC;
using zeebs.utils.zoopBoot;

namespace zeebs.utils.commands
{
	public class Join : Command
	{
		string emoteName;
		bool isAvatar;

		public Join()
		{
			CommandName = "join";
			helptext = "Join - ";

		}
		public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
		{
			base.CanExecute(args, commandParams, emotes);
			if (Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
			{
				FailReasonMessage = "Already part of the game";
				return false;
			}
			if (Utility.GamePlayers.Count == Utility.MainConfig.MaxPlayers)
			{
				FailReasonMessage = "Too many players connected";
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
			FP.World.BroadcastMessage(JoinGameMessage.JoinGame, Args[(int)StdExpMessageValues.UseName], Args[(int)StdExpMessageValues.DisplayName], emoteName, isAvatar, Args[(int)StdExpMessageValues.UserColor]);
		}

		public override Command CreateNewSelf()
		{
			return new Join();
		}

		public enum JoinGameMessage
		{
			JoinGame
		}
		
		public override string GetHelpText() {
			return helptext;
		}
	}
}
