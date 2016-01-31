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
			Utility.Twitchy = new Tankooni.IRC.TwitchInterface("zoopboot", DontLook.logi);
			Utility.Twitchy.Connect("#tankooni");
			Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/tags");
			Library.LoadProvider(new Indigo.Content.TwitchEmoteProvider());

			if (!Directory.Exists("./save/twitchUserData"))
				Directory.CreateDirectory("./save/twitchUserData");
			if (!File.Exists(MainConfig.MainConfigPath))
				Utility.MainConfig = MainConfig.WriteDefaultConfig();
			else
				Utility.MainConfig = MainConfig.LoadMainConfig();

			FP.Console.Enable();
			FP.Console.MirrorToSystemOut = true;
			FP.Console.ToggleKey = Keyboard.Tilde;
			FP.Screen.ClearColor = new Color(0x000000);
			Mouse.CursorVisible = false;

			SoundManager.Init(0.7f);
			//SoundManager.Init(0);
			//FP.World = new DynamicSceneWorld();
			FP.World = new StartScreenWorld();
		}
	}
}
