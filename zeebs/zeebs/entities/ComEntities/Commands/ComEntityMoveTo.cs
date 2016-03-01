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
		bool movingWithDeathInMind = false;
		protected Point moveTo;
		protected Point moveDirection;
		string moverUserName;

		public ComEntityMoveTo(ComEntity comEntity, Point moveTo, bool movingWithDeathInMind, string moverUserName = null)
			: base(comEntity)
		{			
			this.moveTo = moveTo;
			this.moverUserName = moverUserName;
			this.movingWithDeathInMind = movingWithDeathInMind;
		}

		/// <summary>
		/// Init this after setting moveTo
		/// </summary>
		public override void Init()
		{
			base.Init();
			moveDirection = new Point(moveTo.X - comEntity.X, moveTo.Y - comEntity.Y).Normalized();
		}

		public override bool IsDone()
		{
			return isKill || FP.Distance(comEntity.X, comEntity.Y, moveTo.X, moveTo.Y) < 1;
		}
		
		public override IEnumerator Update()
		{
			//set temp vars before actually moving the object
			var newX = comEntity.X;
			var newY = comEntity.Y;
			Indigo.Utils.Approach.TowardsWithDecay(ref newX, moveTo.X);
			Indigo.Utils.Approach.TowardsWithDecay(ref newY, moveTo.Y);

			if(FP.World.CollidePoint("ClickMap", newX, newY) != null || newX < 0 || newX > FP.Width || newY < 0 || newY > FP.Height)
			{
				isKill = true;
				if (movingWithDeathInMind)
				{
					FP.World.BroadcastMessage(
						StartScreenWorld.WorldMessages.PlayerKilledPlayer,
						comEntity.TwitchUserComEntityData.TwitchUserName,
						moverUserName ?? comEntity.TwitchUserComEntityData.TwitchUserName);
					yield return null;
				}

				if (newX < 0)
					newX = 0;
				else if (newX > FP.Width)
					newX = FP.Width;
				if (newY < 0)
					newY = 0;
				else if (newY > FP.Height)
					newY = FP.Height;

				
				for (int i = 1; FP.World.CollidePoint("ClickMap", newX, newY) != null; i++)
				{
					var moveBack = (moveDirection * i);
					newX -= moveBack.X;
					newY -= moveBack.Y;
				}
			}

			comEntity.X = newX;
			comEntity.Y = newY;
			

			//Console.WriteLine(comEntity.MoveTo(x, y/*, movingWithDeathInMind ? null : "ClickMap"*/));
			//Console.WriteLine("x1: " + x + ", y1: " + y + ", x2: " + comEntity.X + ", y2: " + comEntity.Y);
			//Console.WriteLine("Distance: " + FP.Distance(comEntity.X, comEntity.Y, moveTo.X, moveTo.Y));

			//if ((isKill || FP.World.CollidePoint("ClickMap", comEntity.X, comEntity.Y) != null) && movingWithDeathInMind)
			//{
			//	isKill = true;
			//	FP.World.BroadcastMessage(
			//		StartScreenWorld.WorldMessages.PlayerKilledPlayer,
			//		comEntity.TwitchUserComEntityData.TwitchUserName,
			//		moverUserName ?? comEntity.TwitchUserComEntityData.TwitchUserName);
			//}
			yield return null;
		}
	}
}
