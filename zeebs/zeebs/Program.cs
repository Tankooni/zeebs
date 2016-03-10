using System;
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

namespace zeebs
{
	class Game : Engine
	{
		static void Main(string[] args)
		{
			
			var game = new Game();
			game.Run();
		}

		public Game() :
			base(1280, 720, 60)
		{
			if (!Directory.Exists("./save/twitchUserData"))
				Directory.CreateDirectory("./save/twitchUserData");
			if (!File.Exists(MainConfig.MainConfigPath))
				Utility.MainConfig = MainConfig.WriteDefaultConfig();
			else
				Utility.MainConfig = MainConfig.LoadMainConfig();

			Utility.Twitchy = new Tankooni.IRC.TwitchInterface(Utility.MainConfig.Channel, Utility.MainConfig.OverrideBotUser, Utility.MainConfig.OverrideOauth, Utility.MainConfig.IsDebug, Utility.MainConfig.IsOfflineMode);

			var emoteProvider = new Indigo.Content.TwitchEmoteProvider();
			Library.LoadProvider(emoteProvider);

			if (Utility.MainConfig.IsDebug)
			{
				FP.Console.Enable();
				FP.Console.MirrorToSystemOut = true;
				FP.Console.ToggleKey = Keyboard.Tilde;
			}
			var match = Regex.Match(Utility.MainConfig.BackgroundColor, "#?([A-Fa-f0-9]{6}|random)");
			FP.Screen.ClearColor = new Color(int.Parse(match.Success ? match.Groups[1].Value : "000000", System.Globalization.NumberStyles.HexNumber));

			Mouse.CursorVisible = true;

			SoundManager.Init(0.7f);
			//SoundManager.Init(0);
			//FP.World = new DynamicSceneWorld();
			FP.World = new StartScreenWorld(Utility.Twitchy);

			Utility.Twitchy.Connect();
			//Utility.Twitchy.Connect("#tankooni");
			Utility.Twitchy.SendPublicCommand("CAP", "REQ", "twitch.tv/tags");
			Utility.Twitchy.SendPublicCommand("CAP", "REQ", "twitch.tv/membership");
			Utility.Twitchy.SendPublicCommand("CAP", "REQ", "twitch.tv/commands");
			Utility.Twitchy.SendPrivateCommand("CAP", "REQ", "twitch.tv/tags");
			Utility.Twitchy.SendPrivateCommand("CAP", "REQ", "twitch.tv/membership");
			Utility.Twitchy.SendPrivateCommand("CAP", "REQ", "twitch.tv/commands");
		}
	}
}
