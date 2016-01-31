using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using zeebs.utils.commands;
using Tankooni;

namespace Tankooni.IRC
{
	public enum RegexTypes
	{
		StdExpMessage
	}
	public class TwitchInterface
	{
        public static TwitchInterface MasterTwitchInterface;
		IRC Irc;
		Thread IrcThread;
		string channel;
		string nickName;
		string oauth;
		float messageOutRate = (30.0f / 100.0f);

		Dictionary<RegexTypes, Regex> regExers = new Dictionary<RegexTypes, Regex>
		{
			{
				RegexTypes.StdExpMessage,
				new Regex(@"@color=(?:#[A-F0-9]{6}){0,1};display-name=([\w\W]*);emotes=([\w\W]*);mod=([01]);room-id=(\d+);subscriber=([01]);turbo=([01]);user-id=(\d+);user-type=([\w\W]+):([\w\W]+)\!(?:[\w\s\S]+)(?:[\w\W\s])\.tmi\.twitch\.tv\s(PRIVMSG)\s#([\w\s]+)\s+:([\w\W\s\S]+)")
			}
		};

		public static Dictionary<string, Command> commandBank = new Dictionary<string, Command>();

		public TwitchInterface(string nickName, string oauth)
		{
            MasterTwitchInterface = this;
			this.nickName = nickName;
			this.oauth = oauth;
			IrcThread = new Thread(() => { while (true) { Irc.Update(); Thread.Sleep(100); } });
			IrcThread.IsBackground = true;
			Irc = new IRC("irc.twitch.tv", 6667, nickName, oauth);
			Irc.CommandReceiveCallBack = OmgImSoPopular;

			foreach(var commandType in Tankooni.Utility.GetTypeFromAllAssemblies<Command>())
			{
				if (commandType.IsAbstract)
					continue;
				var ctor = commandType.GetConstructor(Type.EmptyTypes);
				var command = (Command)ctor.Invoke(null);
				commandBank.Add(command.CommandName, command);
			}
		}

		public void Connect(string channel)
		{
			this.channel = channel;

			Irc.Connect();
			Irc.Join(channel);
			IrcThread.Start();
		}

		public void OmgImSoPopular(string message)
		{
			Match match = regExers[RegexTypes.StdExpMessage].Match(message);
			if (match.Success)
			{
				if(match.Groups[12].Value.StartsWith("!"))
				{
					var maybeCommand = Regex.Match(match.Groups[12].Value, @"\!(\w+)\s*").Groups[1].Value;
					Command command;
					if (commandBank.TryGetValue(maybeCommand.ToLower(), out command))
					{
						var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
						string failMessage;
						if (command.CanExecute(args, out failMessage))
							command.Execute(args);
						if(!String.IsNullOrWhiteSpace(failMessage))
						{
							SendMessageToServer("@" + args[1] + ": " + failMessage);
						}
					}
				}
			}
		}

		public void SendMessageToServer(string message)
		{
			if (Irc.Connected)
				Irc.SendData("PRIVMSG", channel + " :" + message);
		}

		public void SendPriveMessageToServer(string user, string message)
		{
			if (Irc.Connected)
				Irc.SendData("PRIVMSG", user + " : " + message);
		}

		public void SendCommand(string one, string two, string three)
		{
			Irc.SendData(one, two + " :" + three);
		}

		//public void 

		public void CloseConnection()
		{
			IrcThread.Abort();
			Irc.Close();
		}
	}
}
