using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zeebs.metaData;
using Indigo.Components;
using Indigo.Graphics;
using zeebs.utils.commands;
using Indigo;
using Indigo.Core;

namespace zeebs.entities
{

	public class ComEntity : AnimatedEntity
	{
		public readonly TwitchUserComEntityData TwitchUserComEntityData;
		CoroutineHost coHostCommands = new CoroutineHost();
		//public Emitter emitter;
		public ComEntity(TwitchUserComEntityData twitchUserComEntityData)
			: base
			(
				  twitchUserComEntityData.ComEntityName, 
				  twitchUserComEntityData.ComEmoteHead, 
				  String.IsNullOrWhiteSpace(twitchUserComEntityData.TwitchUserColor) ? 
					Color.White : 
					new Color(int.Parse(twitchUserComEntityData.TwitchUserColor, System.Globalization.NumberStyles.HexNumber)))
		{
			this.CenterOrigin();
			TwitchUserComEntityData = twitchUserComEntityData;
			X = twitchUserComEntityData.ComEntityPosition.X;
			Y = twitchUserComEntityData.ComEntityPosition.Y;
			AddComponent(coHostCommands);
			AddResponse(Attack.AttackeMessage.Attack, DoReceiveAttack);
			//try
			//{
			//	emitter = new Emitter(Library.GetTexture("twitch//" + twitchUserComEntityData.ComEmoteHead));
			//}
			//catch
			//{

			//}
			//if (emitter != null)
			//{
			//	emitter.Relative = false;
			//	var myType = emitter.Define("trail");
			//	myType.Alpha.From = .3f;
			//	myType.Alpha.To = 0;
			//	//myType.Scale.From = 1;
			//	//myType.Scale.To = 0;
			//	myType.Lifetime.Duration = 0.5f;
			//	myType.Motion.Angle = 0;
			//	myType.Motion.AngleVariance.Add = 360;
			//	myType.Motion.Distance = 30;
			//	myType.Motion.DistanceVariance.Add = 4;
			//	AddComponent(emitter);
			//}
			
		}

		public void QueueCommand(ComEntityCommand command)
		{
			TwitchUserComEntityData.CommandQueue.Enqueue(command);
			if (!coHostCommands.Running)
				coHostCommands.Start(DoCommandQueue(TwitchUserComEntityData.CommandQueue));
		}

        public int CountInQueue()
        {
            return TwitchUserComEntityData.CommandQueue.Count;
        }

		public override void Update()
		{
			base.Update();
		}

		public IEnumerator DoCommandQueue(Queue<ComEntityCommand> commands)
		{
			while (commands.Count != 0)
			{
				var command = commands.Peek();
				command.Init();
				while (!command.IsDone())
				{
					yield return command.Update();
				}
				commands.Dequeue();
			}
		}

		public void DoReceiveAttack(object[] args)
		{
			string userName = (string)args[0];
			if (userName == TwitchUserComEntityData.TwitchUserName)
				return;
			ComEntity attacker = (ComEntity)args[1];
			
			if (FP.Distance(X, Y, attacker.X, attacker.Y) < 60)
			{
				Interrupt();
				//hitVector
				QueueCommand(new ComEntityMoveTo(this, (new Point(X, Y) + new Point(X - attacker.X, Y - attacker.Y).Normalized() * 80), userName));
			}
		}

		public void Interrupt()
		{
			if (TwitchUserComEntityData.CommandQueue.Count != 0)
				TwitchUserComEntityData.CommandQueue.Peek().Interrupt();
			TwitchUserComEntityData.CommandQueue.Clear();
			if (coHostCommands.Running)
				coHostCommands.StopAll();
		}
	}
}
