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
using Indigo.Masks;
using Tankooni.Pathing;

namespace zeebs
{
	class StartScreenWorld : World
	{
		private Text start;
		private Text instructions;
		private readonly int TileSize = 16;
		private readonly PathNode[,] pathNodes;
		private readonly Grid nodeGrid;
		private readonly Entity nodeGridEntity;
		private readonly LeaderBoard leaderBoard;

		public TwitchInterface twitchy;

		public StartScreenWorld(TwitchInterface twitchInterface)
		{
			twitchy = twitchInterface;
			if (Utility.MainConfig.UseBackgroundImage)
				AddGraphic(new Image(Library.GetTexture("content/Background.png")));

			leaderBoard = new LeaderBoard
			{
				Y = 60
			};
			if(Utility.MainConfig.ShowLeaderBoard)
				Add(leaderBoard);

			AddResponse(Join.JoinGameMessage.JoinGame, DoJoinGame);
			AddResponse(Leave.LeaveMessage.Leave, DoPartGame);
			AddResponse(Move.MoveMessage.Move, DoMoveZeeb);
            AddResponse(MoveD.MoveDMessage.MoveD, DoMoveDZeeb);
            AddResponse(Loop.LoopMessage.Loop, DoLoop);
			AddResponse(Change.ChangeMessage.Change, DoChangeEmote);
            AddResponse(Spin.SpinMessage.Spin, DoSpinZeeb);
			AddResponse(ChangeColor.ChangeColorMessage.ChangeColor, DoChangeColor);
			AddResponse(Flip.FlipMessage.Flip, DoFlipZeeb);
			AddResponse(Cancel.CancelMessage.Cancel, DoCancelCommands);
			AddResponse(Attack.AttackeMessage.Attack, DoAttack);
      AddResponse(Help.HelpMessage.Help, DoHelpZeeb);

			AddResponse(WorldMessages.PlayerKilledPlayer, DoPlayerKillPlayer);
			AddResponse(WorldMessages.UpdateLeaderBoard, UpdateLeaderBoard);

			AddResponse(SaveScores.SaveScoresMessage.SaveScores, WriteAllScores);

			start = new Text("Start [Enter]");
			start.X = (FP.Width / 2) - (start.Width / 2);
			start.Y = (FP.Height / 3) + 25;

			instructions = new Text("Instructions [Space]");
			instructions.X = (FP.Width / 2) - (instructions.Width / 2);
			instructions.Y = (FP.Height / 3) + 50;


			nodeGridEntity = new Entity();
			nodeGridEntity.AddComponent<Grid>(nodeGrid = new Grid(FP.Width, FP.Height, TileSize, TileSize));
			nodeGridEntity.Type = "ClickMap";

			pathNodes = new PathNode[FP.Width / TileSize, FP.Height / TileSize];
			for (int x = 0; x < pathNodes.GetLength(0); x++)
				for (int y = 0; y < pathNodes.GetLength(1); y++)
					pathNodes[x, y] = new PathNode(null, x * TileSize + TileSize / 2, y * TileSize + TileSize / 2, false);
			//for (int x = 0; x < pathNodes.GetLength(0); x++)
			//	for (int y = 0; y < pathNodes.GetLength(1); y++)
			//		PathNode.ConnectedNodes[pathNodes[x, y]] = SolverUtility.SelectTilesAroundTile(x, y, pathNodes);

			Utility.LoadAndProcessClickMap("content/MoveMap.png", pathNodes, nodeGrid, TileSize);
			Add(nodeGridEntity);
			//AddGraphic(start);
			//JsonWriter.Save(
			//new AnimatedEntityData
			//{
			//	Animations = new List<string> { "ZeebIdle" },
			//	DefaultAnimation = "ZeebIdle"

			//}, "MetaData");
		}

		public void DoJoinGame(object[] args)
		{
			string userName = (string)args[0];
			string displayName = (string)args[1];
			string emoteName = (string)args[2];
			bool isAvatar = (bool)args[3];
			string userColor = (string)args[4];

			//string pathName = "./" + Utility.SAVE_DIR + "/" + Utility.TWITCH_SAVE_DIR + "/" + userName + JsonLoader.RESOURCE_EXT;
			TwitchUserComEntityData userData;
			int dX;
			int dY;
			do
			{
				dX = FP.Random.Int(0, FP.Width);
				dY = FP.Random.Int(0, FP.Height);
			} while (FP.World.CollidePoint("ClickMap", dX, dY) != null);

			//if (!File.Exists(pathName))
			//{
				//JsonWriter.Save(userData, pathName, false);
				//new ComEntity()
			//}
			//else
			//{
			//	userData = JsonLoader.Load<TwitchUserComEntityData>(pathName, false);
			//	userData.ComEmoteHead = emoteName;
			//	userData.ComEntityPosition = new Point(dX, dY);
			//}

			ComEntity newPlayer;
			if (!Utility.SessionPlayers.ContainsKey(userName))
			{
				userData = new TwitchUserComEntityData
				{
					TwitchUserName = userName,
					TwitchDisplayName = displayName,
					TwitchUserColor = string.IsNullOrWhiteSpace(userColor) ? String.Format("{0:X6}", FP.Random.Int(16777216)) : userColor,
					ComEmoteHead = emoteName,
					ComEmoteHeadIsAvatar = isAvatar,
					ComEntityName = Utility.MainConfig.DefaultBody ?? "ZeebSmall",
					ComEntityPosition = new Point(dX, dY),
					CommandQueue = new Queue<ComEntityCommand>()
				};
				newPlayer = new ComEntity(userData);
				Utility.SessionPlayers.Add(userName, newPlayer);
			}
			else
			{
				newPlayer = Utility.SessionPlayers[userName];
				newPlayer.ChangeHead(emoteName, isAvatar);
				newPlayer.TwitchUserComEntityData.ComEntityPosition = new Point(dX, dY);
				newPlayer.X = dX;
				newPlayer.Y = dY;
			}

			Utility.GamePlayers.Add(userName, newPlayer);

			Add(newPlayer);

			BroadcastMessage(WorldMessages.UpdateLeaderBoard);
		}

		public void DoChangeEmote(object[] args)
		{
			var player = Utility.GamePlayers[(string)args[0]];
			player.QueueCommand(new ComEntityChangeHead(player, (string)args[1], (bool)args[2]));
		}

		public void DoPartGame(object[] args)
		{
			string userName = (string)args[0];
			//string path = "./" + Utility.SAVE_DIR + "/" + Utility.TWITCH_SAVE_DIR + "/" + userName + JsonLoader.RESOURCE_EXT;
			var discoPlayer = Utility.GamePlayers[userName];
			discoPlayer.TwitchUserComEntityData.ComEntityPosition.X = discoPlayer.X;
			discoPlayer.TwitchUserComEntityData.ComEntityPosition.Y = discoPlayer.Y;
			//JsonWriter.Save(discoPlayer.TwitchUserComEntityData, path, false);
			discoPlayer.Interrupt();
			Remove(discoPlayer);
			Utility.GamePlayers.Remove(userName);
		}

		public void DoMoveZeeb(object[] args)
		{
			var player = Utility.GamePlayers[(string)args[0]];
            player.QueueCommand(new ComEntityMoveTo(player, new Point((int)(args[1]), (int)(args[2])), false));
		}

        public void DoMoveDZeeb(object[] args)
        {
            var player = Utility.GamePlayers[(string)args[0]];
            player.QueueCommand(new ComEntityMoveD(player, new Point((int)(args[1]), (int)(args[2])), false));
        }

        public void DoLoop(object[] args)
        {
			var player = Utility.GamePlayers[(string)args[0]];
			player.QueueCommand(new ComEntityLoop(player, (List<Command>)args[2], (string[])args[1]));
        }

        public void DoSpinZeeb(object[] args)
        {
            var player = Utility.GamePlayers[(string)args[0]];
            player.QueueCommand(new ComEntitySpin(player));
        }

        public void DoHelpZeeb(object[] args)
        {
            var player = Utility.GamePlayers[(string)args[0]];
            player.QueueCommand(new ComEntityHelp(player));
        }

        public void DoFlipZeeb(object[] args)
        {
			try
			{
				var player = Utility.GamePlayers[(string)args[0]];
				player.QueueCommand(new ComEntityFlip(player));
			}
			catch
			{

			}
        }

		public void DoChangeColor(object[] args)
		{
			var player = Utility.GamePlayers[(string)args[0]];
			player.QueueCommand(new ComEntityChangeColor(player, (string)args[1]));
		}

		public void DoPlayerKillPlayer(object[] args)
		{
			var murderer = Utility.GamePlayers[(string)args[1]];
			var victim = Utility.GamePlayers[(string)args[0]];
			long kills = murderer.TwitchUserComEntityData.KillCount;
			if (args[1] != args[0])
				kills = ++murderer.TwitchUserComEntityData.KillCount;
			else
				kills = --murderer.TwitchUserComEntityData.KillCount;

			victim.ResetDamage();

			twitchy.QueuePublicChatMessage(String.Format("{0} has destroyed {1}, {0} has {2} kills", args[1], args[0], kills));
			DoPartGame(args);
			BroadcastMessage(WorldMessages.UpdateLeaderBoard);
		}

		public void DoCancelCommands(object[] args)
		{
			Utility.GamePlayers[(string)args[0]].Interrupt();
		}

		public void DoAttack(object[] args)
		{
			var player = Utility.GamePlayers[(string)args[0]];
			player.QueueCommand(new ComEntityAttack(player));
		}

		public void UpdateLeaderBoard(object[] args)
		{
			leaderBoard.UpdateLeaderBoard();
		}

		public void WriteAllScores(object[] args)
		{
			StringBuilder scoreBuilder = new StringBuilder();
			foreach(var player in Utility.SessionPlayers.Values.OrderByDescending(x => x.TwitchUserComEntityData.KillCount))
			{
				scoreBuilder.AppendLine(player.TwitchUserComEntityData.TwitchUserName + ": " + player.TwitchUserComEntityData.KillCount);
			}
			File.WriteAllText(Utility.SAVE_DIR + "/" + "Scores_" + DateTime.Now.ToString("MMddyy_HHmmss") + ".txt", scoreBuilder.ToString());
		}

		int currentTester = 0;
		string currentDemoZeeb = "Test";
		

		public override void Update()
		{
			base.Update();
			/*
			if (Utility.MainConfig.IsDebug && FP.Focused)
			{
				#region this switches control
				if (Keyboard.Num0.Pressed)
					currentTester = 0;
				if (Keyboard.Num1.Pressed)
					currentTester = 1;
				if (Keyboard.Num2.Pressed)
					currentTester = 2;
				if (Keyboard.Num3.Pressed)
					currentTester = 3;
				if (Keyboard.Num4.Pressed)
					currentTester = 4;
				if (Keyboard.Num5.Pressed)
					currentTester = 5;
				if (Keyboard.Num6.Pressed)
					currentTester = 6;
				if (Keyboard.Num7.Pressed)
					currentTester = 7;
				if (Keyboard.Num8.Pressed)
					currentTester = 8;
				if (Keyboard.Num9.Pressed)
					currentTester = 9;
				#endregion this switches control
				if (Keyboard.Space.Pressed)
				{

					Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes=25:6-10;mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1}",
						(currentDemoZeeb + currentTester),
						"!join Kappa"
						));
				}

				//if (Keyboard.L.Pressed)
				//{
				//	int currentEmote = 0;
				//	while (currentEmote < Utility.KILL_ME_PLEASE.Count)
				//	{
				//		var subEmote = Utility.KILL_ME_PLEASE[currentEmote];

				//		Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes={2}:{3}-{4};mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1} {5}",
				//			(currentDemoZeeb + currentTester),
				//			"!change ",
				//			subEmote.image_id,
				//			8,
				//			7 + subEmote.code.Length,
				//			subEmote.code
				//			));

				//		currentEmote++;
				//	}
				//}

				if (Keyboard.Up.Pressed)
				{

					Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes=25:6-10;mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1}",
						(currentDemoZeeb + currentTester),
						"!up"
						));
				}
				if (Keyboard.Down.Pressed)
				{

					Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes=25:6-10;mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1}",
						(currentDemoZeeb + currentTester),
						"!down"
						));
				}
				if (Keyboard.Left.Pressed)
				{

					Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes=25:6-10;mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1}",
						(currentDemoZeeb + currentTester),
						"!left"
						));
				}
				if (Keyboard.Right.Pressed)
				{

					Utility.Twitchy.OmgImSoPopular(string.Format(@"@color=#FF4500;display-name={0};emotes=25:6-10;mod=1;room-id=114267546;subscriber=0;turbo=0;user-id=40916227;user-type=mod :{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #zoopboot :{1}",
						(currentDemoZeeb + currentTester),
						"!right"
						));
				}

			}
			*/
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

		public enum WorldMessages
		{
			PlayerKilledPlayer,
			UpdateLeaderBoard
		}
	}
}
