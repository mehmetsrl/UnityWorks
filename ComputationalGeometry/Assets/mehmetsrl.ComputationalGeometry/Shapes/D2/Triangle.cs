using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures
{
    [System.Serializable]
    public class Triangle : Shape2D
    {
        public Triangle(ref IShapeRenderer renderer):base(ref renderer){}

        public override void InitShape()
        {
            triangles = new int[] { 0, 1, 2 };
            //uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };

            points = new List<Point>(3) { new Point(new Vector(0, 1)), new Point(new Vector(1, -1)), new Point(new Vector(-1, -1)) };

            scaledVerticies = new Vector3[points.Count];

            if (points.Count > 1)
            {
                edges = new List<Edge>(points.Count);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    edges.Add(new Edge(points[i], points[i + 1]));
                }
                edges.Add(new Edge(points[points.Count - 1], points[0]));
            }

            uv = new Vector2[] {
                            new Vector2(Mathf.Repeat(points[0].x, 1), Mathf.Repeat(points[0].y, 1))
                            ,new Vector2(Mathf.Repeat(points[1].x, 1), Mathf.Repeat(points[1].y, 1))
                            ,new Vector2(Mathf.Repeat(points[2].x, 1), Mathf.Repeat(points[2].y, 1))};

        }

        public override float GetArea()
        {
            return 0;
        }

        protected override void UpdateMesh()
        {
            //Do Something
            base.UpdateMesh();
        }


    }
}