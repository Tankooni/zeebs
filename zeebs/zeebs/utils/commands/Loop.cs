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
    class Loop : Command
    {
		List<Command> commands = new List<Command>();

        public Loop()
		{
			CommandName = "loop";
		}
		public override bool CanExecute(string[] args, out string failMessage)
		{
			if (!Utility.ConnectedPlayers.ContainsKey(args[9]))
			{
				failMessage = "Not part of game";
				return false;
			}
            var prematch = Regex.Match(args[12], @"\!loop\s*(.*)").Groups[1].ToString();
            var matchs = Regex.Matches(prematch, @"\!(\w+)\s*([^\!]*)");
			if (matchs.Count == 0)
			{
				failMessage = "Invalid format. Syntax is: !loop <command> <command> ...";
				return false;
			}

            foreach (Match match in matchs)
            {
                string commandStr = match.Groups[1].ToString();

                //only run certain commands or things will break
                if (!(commandStr == "movezeeb"))
                {
                    failMessage = String.Format("Command cannot be looped: {0}", commandStr);
                    return false;
                }

                //get the commands
                Command command;
                if (TwitchInterface.commandBank.TryGetValue(commandStr.ToLower(), out command))
                {
                    string[] localArgs = (string[])args.Clone();
                    localArgs[12] = match.Groups[0].ToString();
                    string localFailMessage;
                    if (command.CanExecute(localArgs, out localFailMessage))
                    {
                        commands.Add(command);
                    } else
                    {
                        if (!String.IsNullOrWhiteSpace(localFailMessage))
                        {
                            TwitchInterface.MasterTwitchInterface.SendMessageToServer("@" + args[1] + ": " + localFailMessage);
                        }

                        failMessage = "";
                        return false;
                    }
                }
            }

			failMessage = "";
			return true;
		}

		public override void Execute(string[] args)
		{
            FP.World.BroadcastMessage(LoopMessage.Loop, args, commands);
		}

		public enum LoopMessage
		{
			Loop
		}
	}
}
