﻿using Indigo.Core;
using Indigo.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.metaData
{
	public class TwitchUserComEntityData
	{
		public string TwitchUserName;
		public string TwitchUserColor;
		public string ComEmoteHead;
		public string ComEntityName;
		public Point ComEntityPosition;
		[JsonIgnore]
		public Queue<zeebs.entities.ComEntityCommand> CommandQueue;

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			CommandQueue = new Queue<entities.ComEntityCommand>();
		}
	}
}
