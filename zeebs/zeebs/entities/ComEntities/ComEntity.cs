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

		public ComEntity(TwitchUserComEntityData twitchUserComEntityData)
			: base
			(
				  twitchUserComEntityData.ComEntityName, 
				  twitchUserComEntityData.ComEmoteHead, 
				  String.IsNullOrWhiteSpace(twitchUserComEntityData.TwitchUserColor) ? 
					Color.White : 
					new Color(int.Parse(twitchUserComEntityData.TwitchUserColor, System.Globalization.NumberStyles.HexNumber)))
		{
			TwitchUserComEntityData = twitchUserComEntityData;
			X = twitchUserComEntityData.ComEntityPosition.X;
			Y = twitchUserComEntityData.ComEntityPosition.Y;
			AddComponent(coHostCommands);
			AddResponse(Attack.AttackeMessage.Attack, DoReceiveAttack);
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
			var Disctance = FP.Distance(X, Y, attacker.X, attacker.Y);




			if (FP.Distance(X, Y, attacker.X, attacker.Y) < 40)
			{
				Console.WriteLine("Hit");
				if (TwitchUserComEntityData.CommandQueue.Count != 0)
				{
					var command = TwitchUserComEntityData.CommandQueue.Peek();
					if (command != null)
						command.Interrupt();
				}
				TwitchUserComEntityData.CommandQueue.Clear();
				if(coHostCommands.Running)
					coHostCommands.StopAll();

				var hitVector = new Point(X - attacker.X, Y - attacker.Y).Normalized();

				hitVector = hitVector * 80;

				//hitVector

				var moveTo = new Point(X, Y) + hitVector;
				QueueCommand(new ComEntityMoveTo(this, moveTo));
			}
		}
	}
}
