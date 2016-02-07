using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glide;
using Indigo;
using Indigo.Core;
using System.Collections;

namespace zeebs.entities
{
	public class ComEntityMoveTo : ComEntityCommand
	{
		bool isKill = false;
		protected Point moveTo;
		string moverUserName;
		public ComEntityMoveTo(ComEntity comEntity, Point moveTo, string moverUserName = null)
			: base(comEntity)
		{			
			this.moveTo = moveTo;
			this.moverUserName = moverUserName;
		}

		public override bool IsDone()
		{
			return isKill || FP.Distance(comEntity.X, comEntity.Y, moveTo.X, moveTo.Y) < 1;
		}

		public override IEnumerator Update()
		{

			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.X, moveTo.X);
			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.Y, moveTo.Y);
			if(FP.World.CollidePoint("ClickMap", comEntity.X, comEntity.Y) == null)
			{
				isKill = true;
				FP.World.BroadcastMessage(StartScreenWorld.WorldMessages.PlayerKilledPlayer, comEntity.TwitchUserComEntityData.TwitchUserName, moverUserName ?? comEntity.TwitchUserComEntityData.TwitchUserName);
			}
			yield return null;
		}
	}
}
