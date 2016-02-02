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

			Utility.Twitchy = new Tankooni.IRC.TwitchInterface(Utility.MainConfig.BotUser, Utility.MainConfig.Oauth, Utility.MainConfig.IsDebug, Utility.MainConfig.IsOfflineMode);
			
			Library.LoadProvider(new Indigo.Content.TwitchEmoteProvider());

			if (Utility.MainConfig.IsDebug)
			{
				FP.Console.Enable();
				FP.Console.MirrorToSystemOut = true;
				FP.Console.ToggleKey = Keyboard.Tilde;

			}
			var match = Regex.Match(Utility.MainConfig.BackgroundColor, "#?([A-Fa-f0-9]{6}|random)");
			FP.Screen.ClearColor = new Color(int.Parse(match.Success ? match.Groups[1].Value : "000000", System.Globalization.NumberStyles.HexNumber));

			Mouse.CursorVisible = false;

			SoundManager.Init(0.7f);
			//SoundManager.Init(0);
			//FP.World = new DynamicSceneWorld();
			FP.World = new StartScreenWorld();

			Utility.Twitchy.Connect(Utility.MainConfig.Channel);
			//Utility.Twitchy.Connect("#tankooni");
			Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/tags");
			Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/membership");
			Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/commands");
		}
	}
}
