using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures
{
	[System.Serializable]
	public class Edge
	{
		public Point StartPoint { get { return start; } }
		public Point EndPoint { get { return end; } }

		[SerializeField]
		string description;

		[SerializeField]
		Point start, end;

		public override string ToString()
		{
			return description;
		}

		public Edge(Point start, Point end)
		{
			this.start = start;
			this.end = end;

			start.LinkToEdge(this);
			end.LinkToEdge(this);

			description = "Edge from " + start.ToString() + " | to " + end.ToString();
		}

	}
}