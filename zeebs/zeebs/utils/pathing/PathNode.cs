using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

namespace Tankooni.Pathing
{
	/// <summary>
	/// Description of PathNode.
	/// </summary>
	public class PathNode : PriorityQueueNode
	{
		/// <summary>
		/// Enter a node, receive a list of nodes connected to node and how much effort it is to travel to that node from input node
		/// </summary>
		public readonly static Dictionary<PathNode, List<Tuple<PathNode, float>>> ConnectedNodes = new Dictionary<PathNode, List<Tuple<PathNode, float>>>();

		public bool Enabled;
		public float X { get; private set; }
		public float Y { get; private set; }

		public PathNode(List<Tuple<PathNode, float>> connections, float x = 0, float y = 0, bool enabled = true)
		{
			ConnectedNodes.Add(this, connections ?? new List<Tuple<PathNode, float>>());
			Enabled = enabled;
			X = x;
			Y = y;
		}
	}
}
