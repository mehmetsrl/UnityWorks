
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace mehmetsrl.ComputationalGeometry.Structures
{
    [System.Serializable]
    public class Quad : Shape2D
    {
        public Quad(ref IShapeRenderer renderer) : base(ref renderer) { }

        public override float GetArea()
        {
            return 0;
        }

        public override void InitShape()
        {
            triangles = new int[] { 0, 1, 2, 3, 4, 5 };

            points = new List<Point>(3) { new Point(new Vector(0, 0)), new Point(new Vector(0, 1)), new Point(new Vector(1, 0)), 
                                            new Point(new Vector(1, 0)), new Point(new Vector(0, 1)), new Point(new Vector(1, 1)) };

            
            edges = new List<Edge>(4);

            edges.Add(new Edge(points[0], points[1]));
            edges.Add(new Edge(points[1], points[5]));
            edges.Add(new Edge(points[5], points[3]));
            edges.Add(new Edge(points[3], points[0]));

            uv = new Vector2[] {
                points[0].Position.v2,
                points[1].Position.v2,
                points[2].Position.v2,
                points[3].Position.v2,
                points[4].Position.v2,
                points[5].Position.v2
            };

        }
    }
}