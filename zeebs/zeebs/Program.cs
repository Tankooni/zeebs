﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Indigo.Inputs;
using Indigo.Graphics;
using Tankooni;
using System.IO;
using System.Text.RegularExpressions;
using SFML;
using zeebs.metaData;
using Indigo.Core;
using Utils.Json;
using Tankooni.IRC;
using WebSocketSharp;

namespace zeebs
{
	class Game : Engine
	{
		
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				ErrorMessage errorMessage = new ErrorMessage
				{
					RefID = Guid.NewGuid(),
					ErrorTimeStamp = DateTime.Now,
					SentToServer = true,
					ExceptionText = ""
				};
				try
				{
					errorMessage.Configuration = Utility.MainConfig.CopyAndScrubSensitiveData();
					errorMessage.ExceptionText = e.ExceptionObject.ToString();
				}
				catch
				{
					errorMessage.Configuration = MainConfig.CreateDefaultConfig();
				}
				try
				{
					if (!errorMessage.Configuration.IsDebug)
					{
						WebSocket imAllEarsJoe = new WebSocket("ws://ec2-52-91-247-241.compute-1.amazonaws.com:20420");
						imAllEarsJoe.Connect();
						imAllEarsJoe.Send(Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage));
						imAllEarsJoe.Close();
					}

				}
				catch
				{
					errorMessage.SentToServer = false;
				}
				if (!Directory.Exists("logs"))
					Directory.CreateDirectory("logs");
				JsonWriter.Save(errorMessage, string.Format("logs/{0}_{1}_ErrorLog", errorMessage.ErrorTimeStamp.ToString("MMddyy_HHmmss"), errorMessage.RefID), true);
				//Environment.Exit(0);
			};

			var game = new Game();
			game.Run();
		}

		public Game() :
			base(1280, 720, 60)
		{
			FP.Screen.Title = "Zeebs";
			if (!Directory.Exists("./save/twitchUserData"))
				Directory.CreateDirectory("./save/twitchUserData");
			if (!File.Exists(MainConfig.MainConfigPath))
				Utility.MainConfig = MainConfig.WriteDefaultConfig();
			else
				Utility.MainConfig = MainConfig.LoadMainConfig();

			var twtichEmoteProvider = new Indigo.Content.TwitchEmoteProvider(Utility.MainConfig.Channel.Remove(0, 1));
			Library.LoadProvider(twtichEmoteProvider);
			Library.LoadProvider(new Indigo.Content.TwitchAvatarProvider());

			Utility.Twitchy = new TwitchInterface(Utility.MainConfig.Channel, Utility.MainConfig.OverrideBotUser, Utility.MainConfig.OverrideOauth, Utility.MainConfig.IsDebug, Utility.MainConfig.IsOfflineMode);
			Utility.Twitchy.SpecialEmotes = twtichEmoteProvider.LoadedSpecialEmotes;

			if (Utility.MainConfig.IsDebug)
			{
				FP.Console.Enable();
				FP.Console.MirrorToSystemOut = true;
				FP.Console.ToggleKey = Keyboard.Tilde;
			}
			var match = Regex.Match(Utility.MainConfig.BackgroundColor, "#?([A-Fa-f0-9]{6})");
			FP.Screen.ClearColor = new Color(int.Parse(match.Success ? match.Groups[1].Value : "000000", System.Globalization.NumberStyles.HexNumber));

			Mouse.CursorVisible = true;

			//SoundManager.Init(0.7f);
			FP.World = new StartScreenWorld(Utility.Twitchy);

			Utility.Twitchy.Connect();
		}
	}

	class ErrorMessage
	{
		public Guid RefID { get; set; }
		public DateTime ErrorTimeStamp { get; set; }
		public string ExceptionText { get; set; }
		/// <summary>
		/// Be sure to clear out senstive data with this
		/// </summary>
		public TankooniConfig Configuration { get; set; }
		public bool SentToServer { get; set; }
	}
}
