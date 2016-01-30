using System;
using System.Collections.Generic;
using System.Linq;
using Indigo.Masks;
using Priority_Queue;

namespace Tankooni.Pathing
{
	class SolverUtility
	{
		public static List<Tuple<PathNode, float>> SelectTilesAroundTile(int centerX, int centerY, PathNode[,] pathNodes)
		{
			int mapWidth = pathNodes.GetLength(0);
			int mapHeight = pathNodes.GetLength(1);
			List<Tuple<PathNode, float>> nodes = new List<Tuple<PathNode, float>>();
			for (int x = -1; x < 2; ++x)
			{
				int cX = centerX + x;
				if (cX >= mapWidth)
					break;
				if (cX < 0)
					continue;
				for (int y = -1; y < 2; ++y)
				{
					int cY = centerY + y;
					if (cY >= mapHeight)
						break;
					if (cY < 0 || (x == 0 && y == 0))
						continue;
					nodes.Add(Tuple.Create<PathNode, float>(pathNodes[cX, cY], (Abs(x) == 1 && Abs(y) == 1) ? 1.41f : 1));
				}
			}
			return nodes;
		}

		public static float CalculateHeuristic(float x1, float y1, float x2, float y2)
		{
			return Abs(x1 - x2) + Abs(y1 - y2);
		}

		public static IEnumerable<PathNode> SelectAstarPath(PathNode startNode, PathNode endNode, PathNode[,] pathNodes)
		{
			if (startNode != endNode)
			{

				HeapPriorityQueue<PathNode> frontier = new HeapPriorityQueue<PathNode>(PathNode.ConnectedNodes.Keys.Count);
				frontier.Enqueue(startNode, 0);

				Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
				Dictionary<PathNode, float> costSoFar = new Dictionary<PathNode, float>();

				cameFrom.Add(startNode, null);
				costSoFar.Add(startNode, 0);


				while (frontier.Count != 0)
				{
					PathNode current = frontier.Dequeue();
					if (current == endNode)
						break;

					//Console.WriteLine("Processing Node " + current.X + " " + current.Y);
					foreach (var next in PathNode.ConnectedNodes[current])
					{
						if (!next.Item1.Enabled) continue;
						if (cameFrom.ContainsKey(next.Item1)) continue;

						float newCost = costSoFar[current] + next.Item2;
						if (!costSoFar.ContainsKey(next.Item1) || newCost < costSoFar[next.Item1])
						{
							costSoFar[next.Item1] = newCost;
							float priority = newCost + CalculateHeuristic(next.Item1.X, next.Item1.Y, endNode.X, endNode.Y);
							frontier.Enqueue(next.Item1, priority);
							cameFrom[next.Item1] = current;
						}
					}
				}
				//Console.WriteLine("Done Next up building");

				var result = new List<PathNode>();
				PathNode currentNode = endNode;

				do
				{
					//Console.WriteLine("Processing Node2 " + currentNode.X + " " + currentNode.Y);
					PathNode next = null;
					if (!cameFrom.TryGetValue(currentNode, out next))
						break;
					yield return (currentNode = next);
				} while (startNode != currentNode);
			}
			else
			{
				yield return startNode;
			}
		}

		public static float Abs(float number)
		{
			return (number < 0 ? -number : number);
		}
		public static int Abs(int number)
		{
			return (number < 0 ? -number : number);
		}
	}
}
