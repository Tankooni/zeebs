﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using zeebs.utils.commands;
using Tankooni;
using Indigo;

namespace Tankooni.IRC
{
	public enum RegexTypes
	{
		StdExpMessage,
		StdPartMessage
	}

	public enum StdExpMessageValues : int
	{
		FullMessage		= 0,
		UserColor		= 1,
		DisplayName		= 2,
		Emotes			= 3,
		IsMod			= 4,
		RoomId			= 5,
		Subscriber		= 6,
		Turbo			= 7,
		UserId			= 8,
		UserType		= 9,
		UseName			= 10,
		Command			= 11,
		RoomName		= 12,
		Message			= 13

	}

	public class TwitchInterface
	{
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
				new Regex(@"@color=#?([A-F0-9]{6}){0,1};display-name=([\w\W]*);emotes=([\w\W]*);mod=([01]);room-id=(\d+);subscriber=([01]);turbo=([01]);user-id=(\d+);user-type=([\w\W]+):([\w\W]+)\!(?:[\w\s\S]+)(?:[\w\W\s])\.tmi\.twitch\.tv\s(PRIVMSG)\s#([\w\s]+)\s+:([\w\W\s\S]+)")
			},
			{
				RegexTypes.StdPartMessage,
				new Regex(@":([\w\W]+)\!(?:[\w\s\S]+)(?:[\w\W\s])\.tmi\.twitch\.tv\s(PART)\s#([\w\s]+)")
			}
		};

		Dictionary<string, Command> commandBank = new Dictionary<string, Command>();
		public Command RetrieveNewCommandFromBank(string commandName)
		{
			Command command;
			if (!commandBank.TryGetValue(commandName.ToLower(), out command))
				return null;
			return command.CreateNewSelf();
		}

		public TwitchInterface(string nickName, string oauth)
		{
			this.nickName = nickName;
			this.oauth = oauth;
			IrcThread = new Thread(() => { while (true) { Irc.Update(); Thread.Sleep(10); } });
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
				if (match.Groups[(int)StdExpMessageValues.Message].Value.StartsWith("!"))
				{
					var maybeCommand = Regex.Match(match.Groups[(int)StdExpMessageValues.Message].Value, @"\!(\w+)\s*").Groups[1].Value;
					Command command;
					if ((command = RetrieveNewCommandFromBank(maybeCommand)) != null)
					{
						var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
						string failMessage;
						if (command.CanExecute(args, out failMessage))
							command.Execute(args);
						if (!String.IsNullOrWhiteSpace(failMessage))
						{
							SendMessageToServer("@" + args[(int)StdExpMessageValues.UseName] + ": " + failMessage);
						}
					}
				}

				//For stress testing
				//if (Utility.ConnectedPlayers.ContainsKey(match.Groups[(int)StdExpMessageValues.UseName].Value))
				//{
				//	var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
				//	string failMessage;
				//	args[(int)StdExpMessageValues.Message] = "!move " + FP.Random.Int(FP.Width) + " " + FP.Random.Int(FP.Height);
				//	var newCommand = commandBank["move"].CreateNewSelf();
				//	if (newCommand.CanExecute(args, out failMessage))
				//		newCommand.Execute(args);
				//	else
				//		Console.WriteLine(failMessage);
				//}
				//else if(Utility.ConnectedPlayers.Count == Utility.MainConfig.MaxPlayers)
				//{
				//	var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
				//	string failMessage;
				//	args[(int)StdExpMessageValues.Message] = "!move " + FP.Random.Int(FP.Width) + " " + FP.Random.Int(FP.Height);
				//	args[(int)StdExpMessageValues.UseName] = Utility.ConnectedPlayers.Keys.ElementAt(FP.Random.Int(Utility.ConnectedPlayers.Keys.Count));
				//	var newCommand = commandBank["move"].CreateNewSelf();
				//	if (newCommand.CanExecute(args, out failMessage))
				//		newCommand.Execute(args);
				//	else
				//		Console.WriteLine(failMessage);
				//}
				//else
				//{
				//	var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
				//	string failMessage;
				//	var match2 = Regex.Match(args[(int)StdExpMessageValues.Emotes], @"(\d+):(\d+)-(\d+)");
				//	if (!match2.Success)
				//	{
				//		args[(int)StdExpMessageValues.Emotes] = "44073:6-11";
				//		args[(int)StdExpMessageValues.Message] = "!join cutFin";
				//	}
				//	else
				//	{
				//		var startPos = int.Parse(match2.Groups[2].Value);
				//		var endPos = int.Parse(match2.Groups[3].Value);
				//		args[(int)StdExpMessageValues.Emotes] = match2.Groups[2] + ":6-" + (6 + endPos - startPos);
				//		args[(int)StdExpMessageValues.Message] = "!join " + args[(int)StdExpMessageValues.Message].Substring(startPos, endPos - startPos + 1);

				//	}
				//	if (commandBank["join"].CanExecute(args, out failMessage))
				//		commandBank["join"].Execute(args);
				//}

			}
			else if ((match = regExers[RegexTypes.StdPartMessage].Match(message)).Success)
			{
				Console.WriteLine("Parting");
				if (match.Groups[2].Value == "PART")
				{
					var args = match.Groups.Cast<Group>().Select(x => x.Value).ToArray();
					string failMessage;
					Command command;
					if ((command = RetrieveNewCommandFromBank("part")) != null)
					{
						if ((command = commandBank["part"]).CanExecute(args, out failMessage))
							command.Execute(args);
					}
				}
			}
		}

		public void SendMessageToServer(string message)
		{
			if (Irc.Connected && !Utility.MainConfig.PreventBotTalking)
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
