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
        String message = "help menu:" + "\n" +
          "!join <emote>: to join game" +  "\n" +
          "!down: to move down one coordinate";
        Console.WriteLine(message);
        if (!Utility.MainConfig.IsOfflineMode)
          Utility.Twitchy.SendMessageToServer(message);
        else
          Console.WriteLine(message);

        Console.WriteLine("COMMAND HELP");

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
  }
}
