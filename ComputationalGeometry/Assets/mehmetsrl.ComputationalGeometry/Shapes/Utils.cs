using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace mehmetsrl.ComputationalGeometry.Structures
{
	public static class Utils
	{
		public static bool CheckLineIntersection(
		Edge e, Vector p1, Vector p2, out Vector intersection)
		{
			return CheckLineIntersection(e.StartPoint.Position, e.EndPoint.Position, p1, p2, out intersection);
		}
		public static bool CheckLineIntersection(
		Edge e1, Edge e2, out Vector intersection)
		{
			return CheckLineIntersection(e1.StartPoint.Position, e1.EndPoint.Position, e2.StartPoint.Position, e2.EndPoint.Position, out intersection);
		}

		public static bool CheckLineIntersection(Vector p1, Vector p2, Vector p3, Vector p4, out Vector intersection)
		{
			float s1_x, s1_y, s2_x, s2_y;
			s1_x = p2[0] - p1[0];
			s1_y = p2[1] - p1[1];
			s2_x = p4[0] - p3[0];
			s2_y = p4[1] - p3[1];

			float s, t;
			s = (-s1_y * (p1[0] - p3[0]) + s1_x * (p1[1] - p3[1])) / (-s2_x * s1_y + s1_x * s2_y);
			t = (s2_x * (p1[1] - p3[1]) - s2_y * (p1[0] - p3[0])) / (-s2_x * s1_y + s1_x * s2_y);

			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
			{
				// Collision detected
				intersection = new Vector(p1[0] + (t * s1_x), p1[1] + (t * s1_y));
				return true;
			}
			intersection = Vector.invalid;
			return false; // No collision
		}
	}
}