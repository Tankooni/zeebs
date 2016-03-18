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
    class Help : Command
    {
        public Help()
    {
      CommandName = "help";
      helptext = "Help - list all the commands here but i'm not going to type them out here because i can probs use the commandbank to list them all in a loop or something, amiright";

    }
    public override bool CanExecute(string[] args, string commandParams, List<Emote> emotes)
    {
      base.CanExecute(args, commandParams, emotes);
      if (!Utility.GamePlayers.ContainsKey(args[(int)StdExpMessageValues.UseName]))
      {
        FailReasonMessage = "Not part of game";
        return false;
      }
      else {
        /*
        TO DO: Display help menu whether or not you're connected to the game
        TO DO: Overly complex to just send help message to server - are there unncessary classes for Help specfically?
         */

        if (!Utility.MainConfig.IsOfflineMode) {
          Command command;
          string commandInQuestion;

          // !help returns help's msg.
          // !help <cmd> returns <cmd>'s helpmsg
          // !help <cmd1> <cmd2> returns <cmd1>'s helpmsg - by only looking at the first cmd
          if (commandParams.Split().Length == 1)
            commandInQuestion = commandParams;
          else
            commandInQuestion = commandParams.Substring(0,commandParams.IndexOf(" "));

          Utility.Twitchy.commandBank.TryGetValue(commandInQuestion, out command);

          // if that command exists and the help text exists
          if (command!=null && command.GetHelpText()!=null) {
            Console.WriteLine("cmd help description:" + command.GetHelpText());
            Utility.Twitchy.QueuePublicChatMessage(command.GetHelpText());
          }
          //invalid command or non -existent command after `!help`
          else {
            Console.WriteLine("this.CommandName:" +  this.CommandName);
            Utility.Twitchy.QueuePublicChatMessage(this.GetHelpText());
          }
        }
      }

      return true;
    }


    public override void Execute()
    {
      FP.World.BroadcastMessage(HelpMessage.Help, Args[(int)StdExpMessageValues.UseName]);
    }

    public override Command CreateNewSelf()
    {
        return new Help();
    }

    public enum HelpMessage
    {
        Help
    }

    public override string GetHelpText() {
      return helptext;
    }
  }
}
