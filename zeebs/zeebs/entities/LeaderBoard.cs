using Indigo;
using Indigo.Graphics;
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

		public LeaderBoard() : this(Color.Black) { }

		public LeaderBoard(Color color)
		{
			float totalHeight = 0;
			for(int i = 0; i < leaderboardTotal; i++)
			{
				var newText = new Text(y: totalHeight)
				{
					Color = color,
					Size = 38
				};
				totalHeight += newText.Height;
				UserText.Add(newText);
				AddComponent<Text>(newText);
			}
		}

		public void UpdateLeaderBoard()
		{
			int index = 0;
			foreach (var entity in Utility.SessionPlayers.Values.OrderByDescending(x => x.TwitchUserComEntityData.KillCount).Take(leaderboardTotal))
			{
				UserText[index].String = entity.TwitchUserComEntityData.TwitchUserName + ": " + entity.TwitchUserComEntityData.KillCount;
				index++;
			}
		}
	}
}
