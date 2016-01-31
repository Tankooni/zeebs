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
using zeebs.utils.commands;
using System.IO;
using zeebs.entities.ComEntities.Commands;

namespace zeebs
{
	class StartScreenWorld : World
	{
		private Text start;
		private Text instructions;

		public StartScreenWorld()
		{
			AddGraphic(new Image(Library.GetTexture("content/Background.png")));

			AddResponse(Emote.EmoteMessage.Emote, DoEmote);
			AddResponse(Join.JoinGameMessage.JoinGame, DoJoinGame);
			AddResponse(Leave.LeaveMessage.Leave, DoPartGame);
			AddResponse(Move.MoveMessage.Move, DoMoveZeeb);
            AddResponse(Loop.LoopMessage.Loop, DoLoop);
			AddResponse(Change.ChangeMessage.Change, DoChangeEmote);
            AddResponse(Spin.SpinMessage.Spin, DoSpinZeeb);

			start = new Text("Start [Enter]");
			start.X = (FP.Width / 2) - (start.Width / 2);
			start.Y = (FP.Height / 3) + 25;

			instructions = new Text("Instructions [Space]");
			instructions.X = (FP.Width / 2) - (instructions.Width / 2);
			instructions.Y = (FP.Height / 3) + 50;
			//AddGraphic(start);
				//JsonWriter.Save(
				//new AnimatedEntityData
				//{
				//	Animations = new List<string> { "ZeebIdle" },
				//	DefaultAnimation = "ZeebIdle"

				//}, "MetaData");
		}

		public AnimatedEntity mine;

		public void DoEmote(object[] args)
		{
			switch (FP.Random.Int(1))
			{
				case 0:
					mine = Add(new AnimatedEntity("ZeebSmall", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
					mine.SetAlpha(0.5f);
					break;
				case 1:
					mine = Add(new AnimatedEntity("Zeeb", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
					break;
				case 2:
					mine = Add(new AnimatedEntity("TheFuck", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
					break;
				case 3:
					mine = Add(new AnimatedEntity("Ko", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
					break;
				default:
					break;
			}
			 //if()
			 //	Add(new AnimatedEntity("ZeebSmall", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
			 //else
			 //	Add(new AnimatedEntity("Zeeb", args[0].ToString()) { X = FP.Random.Float(FP.Width), Y = FP.Random.Float(FP.Height) });
			 //var g = AddGraphic(new Image(Library.GetTexture("twitch//" + args[0])));
			 //g.CenterOrigin();
			 //g.X = FP.Random.Float(FP.Width);
			 //g.Y = FP.Random.Float(FP.Height);
			 //var image = g.GetComponent<Image>();
			 //image.ScaleX = 24.0f / image.Width;
			 //image.ScaleY = 24.0f / image.Height;

		}

		public void DoJoinGame(object[] args)
		{
			string userName = (string)args[0];
			string emoteName = (string)args[1];
			string pathName = "./" + Utility.SAVE_DIR + "/" + Utility.TWITCH_SAVE_DIR + "/" + userName + JsonLoader.RESOURCE_EXT;
			TwitchUserComEntityData userData;
			if (!File.Exists(pathName))
			{
				userData = new TwitchUserComEntityData
				{
					TwitchUserName = userName,
					TwitchUserColor = (string)args[2],
					ComEmoteHead = emoteName,
					ComEntityName = "ZeebSmall",
					ComEntityPosition = new Point(FP.Random.Float(FP.Width), FP.Random.Float(FP.Height)),
					CommandQueue = new Queue<ComEntityCommand>()
				};
				JsonWriter.Save(userData, pathName, false);
				//new ComEntity()
			}
			else
			{
				userData = JsonLoader.Load<TwitchUserComEntityData>(pathName, false);
				userData.ComEmoteHead = emoteName;
			}

			var newPlayer = new ComEntity(userData);
			Utility.ConnectedPlayers.Add(userName, newPlayer);
			Add(newPlayer);
		}

		public void DoChangeEmote(object[] args)
		{
			Utility.ConnectedPlayers[(string)args[0]].ChangeHead((string)args[1]);
		}

		public void DoPartGame(object[] args)
		{
			string userName = (string)args[0];
			string path = "./" + Utility.SAVE_DIR + "/" + Utility.TWITCH_SAVE_DIR + "/" + userName + JsonLoader.RESOURCE_EXT;
			var discoPlayer = Utility.ConnectedPlayers[userName];
			discoPlayer.TwitchUserComEntityData.ComEntityPosition.X = discoPlayer.X;
			discoPlayer.TwitchUserComEntityData.ComEntityPosition.Y = discoPlayer.Y;
			JsonWriter.Save(discoPlayer.TwitchUserComEntityData, path, false);
			Remove(discoPlayer);
			Utility.ConnectedPlayers.Remove(userName);
		}

		public void DoMoveZeeb(object[] args)
		{
			var player = Utility.ConnectedPlayers[(string)args[0]];
			player.QueueCommand(new ComEntityMoveTo(player, new Point((int)args[1], (int)args[2])));
		}

        public void DoLoop(object[] args)
        {
            string[] realArgs = (string[])args[0];
            List<Command> commands = (List<Command>)args[1];
            bool shouldLoop = (bool)args[2];

            var player = Utility.ConnectedPlayers[realArgs[(int)StdExpMessageValues.UseName]];
            player.QueueCommand(new ComEntityLoop(player, commands, realArgs, shouldLoop));
        }

        public void DoSpinZeeb(object[] args)
        {
            var player = Utility.ConnectedPlayers[(string)args[0]];
            player.QueueCommand(new ComEntitySpin(player));
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
