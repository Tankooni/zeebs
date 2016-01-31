﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zeebs.metaData;
using Indigo.Components;
using Indigo.Graphics;

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
		}

		public void QueueCommand(ComEntityCommand command)
		{
			TwitchUserComEntityData.CommandQueue.Enqueue(command);
			if (!coHostCommands.Running)
				coHostCommands.Start(DoCommandQueue(TwitchUserComEntityData.CommandQueue));
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
					command.Update();
					yield return null;
				}
				commands.Dequeue();
			}
		}
	}
}