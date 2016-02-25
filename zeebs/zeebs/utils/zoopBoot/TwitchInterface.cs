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
using Indigo;
using System.Collections.Concurrent;
using zeebs.utils.zoopBoot;

namespace Tankooni.IRC
{
	public enum RegexTypes
	{
		StdExpMessage,
		StdPartMessage,
		AllCommands,
		EmoteDataPassOne,
		EmoteDataPassTwo
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
		public static TwitchInterface MasterTwitchInterface;
		IRC Irc;
		Thread IrcThread;
		Thread PubChatOut;
		string channel;
		string nickName;
		string oauth;
		float messageOutRate = (30.0f / 100.0f);

		public bool IsDebug;
		public bool IsOfflineMode;

		public ConcurrentQueue<string> PublicChatQueue = new ConcurrentQueue<string>();

		public static Dictionary<RegexTypes, Regex> regExers = new Dictionary<RegexTypes, Regex>
		{
			{
				RegexTypes.StdExpMessage,
				new Regex(@"@color=#?([A-F0-9]{6}){0,1};display-name=([\w\W]*);emotes=([\w\W]*);mod=([01]);room-id=(\d+);subscriber=([01]);turbo=([01]);user-id=(\d+);user-type=([\w\W]+):([\w\W]+)\!(?:[\w\s\S]+)(?:[\w\W\s])\.tmi\.twitch\.tv\s(PRIVMSG)\s#([\w\s]+)\s+:([\w\W\s\S]+)")
			},
			{
				RegexTypes.StdPartMessage,
				new Regex(@":([\w\W]+)\!(?:[\w\s\S]+)(?:[\w\W\s])\.tmi\.twitch\.tv\s(PART)\s#([\w\s]+)")
			},
			{
				RegexTypes.AllCommands,
				new Regex(@"\!(\w+)\s*([^\!]*)")
			},
			{
				RegexTypes.EmoteDataPassOne,
				new Regex(@"(\d+):([^/]*)")
			},
			{
				RegexTypes.EmoteDataPassTwo,
				new Regex(@"(\d+)-(\d+)")
			}
		};

		public Dictionary<string, Command> commandBank = new Dictionary<string, Command>();
		public Command RetrieveNewCommandFromBank(string commandName)
		{
			Command command;
			if (!commandBank.TryGetValue(commandName.ToLower(), out command))
				return null;
			return command.CreateNewSelf();
		}

		public TwitchInterface(string nickName, string oauth, bool isDebug = false, bool isOfflineMode = false)
		{
			MasterTwitchInterface = this;
			IsDebug = isDebug;
			IsOfflineMode = isOfflineMode;
			this.nickName = nickName;
			this.oauth = oauth;
			if (!isOfflineMode)
			{
				IrcThread = new Thread(() => { while (true) { Irc.Update(); Thread.Sleep(10); } });
				IrcThread.IsBackground = true;
				Irc = new IRC("irc.twitch.tv", 6667, nickName, oauth, isDebug);
				Irc.CommandReceiveCallBack = OmgImSoPopular;

				PubChatOut = new Thread(() => { while (true) { PublicChatMessageQueueDoer(); Thread.Sleep(100); } });
				PubChatOut.IsBackground = true;
			}
			else
				if (isDebug) Console.WriteLine("Offline Mode enabled");

			foreach (var commandType in Tankooni.Utility.GetTypeFromAllAssemblies<Command>())
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
			PubChatOut.Start();
		}

		//public void SpoofMessage(string )

		public void RunCommand(Command command)
		{

		}

		public void OmgImSoPopular(string message)
		{
			Match messageMatch = regExers[RegexTypes.StdExpMessage].Match(message);
			if (messageMatch.Success)
			{
				if (messageMatch.Groups[(int)StdExpMessageValues.Message].Value.StartsWith("!"))
				{
					var allPotentialCommandMatches = regExers[RegexTypes.AllCommands].Matches(messageMatch.Groups[(int)StdExpMessageValues.Message].Value);
					var allEmotes = regExers[RegexTypes.EmoteDataPassOne].Matches(messageMatch.Groups[(int)StdExpMessageValues.Emotes].Value);
					var emoteQueue = new List<Emote>();
					foreach (Match emoteSet in allEmotes)
					{
						string emoteID = emoteSet.Groups[1].Value;
						foreach (Match emote in regExers[RegexTypes.EmoteDataPassTwo].Matches(emoteSet.Groups[2].Value))
						{
							emoteQueue.Add(new Emote(emoteID, int.Parse(emote.Groups[1].Value), int.Parse(emote.Groups[2].Value)));
						}
					}
					emoteQueue = emoteQueue.OrderBy(x => x.StartPos).ToList();
					var args = messageMatch.Groups.Cast<Group>().Select(x => x.Value).ToArray();
					bool greedIsPresent = false;
					List<Command> commandsToQueue = new List<Command>();
					Command firstFailedCommand = null;
					HashSet<string> failedCommandNames = new HashSet<string>();
					List<Command> hoardedCommands = new List<Command>();

					int currentCommandStartPosition = 0;

					foreach (Match potentialCommand in allPotentialCommandMatches)
					{
						Command newCommand = RetrieveNewCommandFromBank(potentialCommand.Groups[1].Value);
						if (newCommand == null)
						{
							currentCommandStartPosition += potentialCommand.Value.Length;
							while (emoteQueue.Count > 0 && emoteQueue.First().StartPos < currentCommandStartPosition)
								emoteQueue.RemoveAt(0);
							
							continue;
						}

						var commandParamStartPos = currentCommandStartPosition + potentialCommand.Groups[1].Value.Length + 2;
						currentCommandStartPosition += potentialCommand.Value.Length;
						var emoteList = new List<Emote>();

						while (emoteQueue.Count > 0 && emoteQueue.First().StartPos < currentCommandStartPosition)
						{
							Emote emote = emoteQueue.First();
							emoteQueue.RemoveAt(0);
							emote.StartPos -= commandParamStartPos;
							emote.EndPos -= commandParamStartPos;
							emoteList.Add(emote);
						}

						if (newCommand.CanExecute(args, potentialCommand.Groups[2].Value, emoteList))
						{
							if (!greedIsPresent)
								commandsToQueue.Add(newCommand);
							else
								hoardedCommands.Add(newCommand);

							if (!greedIsPresent && newCommand.IsGreedy())
								greedIsPresent = true;
						}

						if (!String.IsNullOrWhiteSpace(newCommand.FailReasonMessage))
						{
							if (firstFailedCommand == null)
								firstFailedCommand = newCommand;
							if (!failedCommandNames.Contains("!" + newCommand.CommandName))
								failedCommandNames.Add("!" + newCommand.CommandName);
						}
							//QueuePublicChatMessage("@" + args[(int)StdExpMessageValues.UseName] + ": " + newCommand.FailReasonMessage);
					}

					if (firstFailedCommand != null)
					{
						string failMessage = "@" + args[(int)StdExpMessageValues.UseName] + ": ";
						if(failedCommandNames.Count() > 1)
						{
							failMessage += "First Fail Message: " + firstFailedCommand.FailReasonMessage;
							failMessage += ", as well as the following commands; " + String.Join(", ", failedCommandNames);
						}
						else
						{
							failMessage += firstFailedCommand.FailReasonMessage;
						}
						QueuePublicChatMessage(failMessage);
						return;
					}

					foreach (var commandToExecute in commandsToQueue)
					{
						if (commandToExecute.IsGreedy())
							commandToExecute.SetCommandList(hoardedCommands);
						commandToExecute.Execute();
					}

				}
			}
			else if ((messageMatch = regExers[RegexTypes.StdPartMessage].Match(message)).Success)
			{
				if(Utility.MainConfig.IsDebug)
					Console.WriteLine("Parting");
				if (messageMatch.Groups[2].Value == "PART")
				{
					var args = messageMatch.Groups.Cast<Group>().Select(x => x.Value).ToArray();
					Command command;
					if ((command = RetrieveNewCommandFromBank("part")) != null)
					{
						if ((command = commandBank["part"]).CanExecute(args, "", null))
							command.Execute();
						if (Utility.MainConfig.IsDebug && !String.IsNullOrWhiteSpace(command.FailReasonMessage))
							Console.WriteLine(command.FailReasonMessage);
					}
				}
			}
		}

		public void QueuePublicChatMessage(string message)
		{
			PublicChatQueue.Enqueue(message);
		}

		public void PublicChatMessageQueueDoer()
		{
			string message;
			if (PublicChatQueue.TryDequeue(out message))
			{
				if (!Utility.MainConfig.IsOfflineMode)
					SendMessageToServer(message);
				else
					Console.WriteLine(message);
			}
		}

		protected void SendMessageToServer(string message)
		{
			if (Irc.Connected && !Utility.MainConfig.PreventBotTalking)
				Irc.SendData("PRIVMSG", channel + " :" + message);
		}

		//public void SendPriveMessageToServer(string user, string message)
		//{
		//	if (Irc.Connected)
		//		Irc.SendData("PRIVMSG", user + " : " + message);
		//}

		public void SendCommand(string one, string two, string three)
		{
			Irc.SendData(one, two + " :" + three);
		}

		public void CloseConnection()
		{
			IrcThread.Abort();
			PubChatOut.Abort();
			Irc.Close();
		}
	}
}
