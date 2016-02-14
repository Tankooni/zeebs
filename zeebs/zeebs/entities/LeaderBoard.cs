﻿using Indigo;
using Indigo.Graphics;
using Indigo.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;

namespace zeebs.entities
{
	public class LeaderBoard : Entity
	{
		int leaderboardTotal = 10;
		public List<Text> UserText = new List<Text>();
		Image background;
		public LeaderBoard() : this(Color.Black) { }

		int minWidth = 0;

		public LeaderBoard(Color color)
		{
			background = new Image(Library.GetTexture("content/white.png"))
			{
				Alpha = .8f,
				Color = new Color(0xEEFFEE)
			};
			AddComponent(background);
			
			var header = new Text()
			{
				Color = Color.Black,
				String = "Leaderboard",
				Size = 40,
				Bold = true
			};
			UserText.Add(header);
			AddComponent(header);

			float totalHeight = header.Height;
			for(int i = 1; i < leaderboardTotal; i++)
			{
				var newText = new Text(y: totalHeight)
				{
					Size = 38
				};
				totalHeight += newText.Height;
				UserText.Add(newText);
				AddComponent<Text>(newText);
			}
			background.ScaleY = minWidth = Height = (int)Math.Ceiling(totalHeight);
			background.ScaleX = Width = (int)Math.Ceiling(header.Width);
		}

		public void UpdateLeaderBoard()
		{
			int index = 1;
			float longestWidth = minWidth;
			foreach (var entity in Utility.SessionPlayers.Values.OrderByDescending(x => x.TwitchUserComEntityData.KillCount).Take(leaderboardTotal))
			{
				var currentText = UserText[index];
				string userName = entity.TwitchUserComEntityData.TwitchUserName;
				if (userName.Length > 10)
					userName = new string(userName.Take(10).ToArray()) + "...";
				currentText.Color = new Color(int.Parse(entity.TwitchUserComEntityData.TwitchUserColor, System.Globalization.NumberStyles.HexNumber));
				currentText.String = userName + ": " + entity.TwitchUserComEntityData.KillCount;
				if (currentText.Width > longestWidth)
					longestWidth = currentText.Width;
				index++;
			}

			background.ScaleX = Width = (int)Math.Ceiling(longestWidth);
		}
		bool MouseHeld = false;
		public override void Update()
		{
			base.Update();
			DoMouseThings();
			//if()
		}

		public void DoMouseThings()
		{
			if (Mouse.Left.Pressed && CollidePoint(X, Y, Mouse.ScreenX, Mouse.ScreenY))
			{
				MouseHeld = true;
				return;
			}
			if (Mouse.Left.Released)
			{
				MouseHeld = false;
				return;
			}

			if (MouseHeld)
			{
				this.X += Mouse.DeltaX;
				this.Y += Mouse.DeltaY;
			}

			
		}
	}
}
