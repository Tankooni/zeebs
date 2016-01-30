using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Indigo.Inputs;
using Indigo.Graphics;
using Tankooni.IRC;
using Tankooni;
using zeebs.metaData;
using Indigo.Core;
using Utils.Json;
using zeebs.entities;

namespace zeebs
{
	class StartScreenWorld : World
	{
		private Text start;
		private Text instructions;

		public StartScreenWorld()
		{
			AddGraphic(new Image(Library.GetTexture("content/Background.png")));
			AddResponse(StringSplitOptions.RemoveEmptyEntries, DoEmote);
			start = new Text("Start [Enter]");
			start.X = (FP.Width / 2) - (start.Width / 2);
			start.Y = (FP.Height / 3) + 25;

			instructions = new Text("Instructions [Space]");
			instructions.X = (FP.Width / 2) - (instructions.Width / 2);
			instructions.Y = (FP.Height / 3) + 50;
			AddGraphic(start);
				//JsonWriter.Save(
				//new AnimatedEntityData
				//{
				//	Animations = new List<string> { "ZeebIdle" },
				//	DefaultAnimation = "ZeebIdle"

				//}, "MetaData");
		}

		public void DoEmote(object[] args)
		{
			Add(new AnimatedEntity("Zeeb", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
			//var g = AddGraphic(new Image(Library.GetTexture("twitch//" + args[0])));
			//g.CenterOrigin();
			//g.X = FP.Random.Float(FP.Width);
			//g.Y = FP.Random.Float(FP.Height);
			//var image = g.GetComponent<Image>();
			//image.ScaleX = 24.0f / image.Width;
			//image.ScaleY = 24.0f / image.Height;
			
		}

		public override void Update()
		{
			base.Update();
			//if (Keyboard.A.Pressed)
			//	Utility.Twitchy.SendMessageToServer("Hai frondsww");
			//if (Keyboard.S.Pressed)
			//	Utility.Twitchy.SendPriveMessageToServer("chjolo", "Hai frond");

			//if (Keyboard.Z.Pressed)
			//	Utility.Twitchy.OmgImSoPopular("@color=#FF4500;display-name=Tankooni;emotes=44073:0-5/44355:7-12;mod=0;room-id=40916227;subscriber=0;turbo=0;user-id=40916227;user-type= :tankooni!tankooni@tankooni.tmi.twitch.tv PRIVMSG #tankooni :cutFin cutBoy");

			//if (Keyboard.Q.Pressed)
			//	Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/membership");
			//if (Keyboard.W.Pressed)
			//	Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/commands");
			//if (Keyboard.E.Pressed)
			//	Utility.Twitchy.SendCommand("CAP", "REQ", "twitch.tv/tags");
			//	FP.World = new DynamicSceneWorld(Utility.MainConfig.StartingScene, Utility.MainConfig.SpawnEntrance);

			//            if (Keyboard.Space.Pressed)
			//                FP.World = new InstructionsScreenWorld();
		}
	}
}
