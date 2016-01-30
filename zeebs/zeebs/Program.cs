using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Indigo.Inputs;
using Indigo.Graphics;

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
			base(1280, 780, 60)
		{
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
