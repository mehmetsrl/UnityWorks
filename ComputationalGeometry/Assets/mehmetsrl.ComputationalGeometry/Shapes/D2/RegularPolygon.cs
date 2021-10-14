using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures
{
    [System.Serializable]
    public class RegularPolygon : Shape2D
    {
        public RegularPolygon(ref IShapeRenderer renderer) : base(ref renderer) { }

        [Range(3, byte.MaxValue)]
        public byte edgeNumber =3;
        public float radius = 0.5f;

        private float angleStep = 0;

        private Point center;
        private bool addCenter = false;

        public override float GetArea()
        {
            return 0;
        }

        public override void InitShape()
        {

            center = new Point(new Vector(0,0));

            angleStep = (360 / edgeNumber);

            triangles = new int[edgeNumber];
            for (int i = 0; i < edgeNumber; i++)
            {
                triangles[i] = i;
            }

            points = new List<Point>(edgeNumber);
            for (int i = 0; i < edgeNumber; i++)
            {
                float angle = angleStep * i;
                points.Add(
                    new Point(
                    new Vector(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle)
                    )));
                if (addCenter)
                {
                    points.Add(center);
                }

                addCenter = !addCenter;

            }

            addCenter = false;

            edges = new List<Edge>();
            for (int i = 0; i < points.Count - 1; i++)
            {

                edges.Add(new Edge(points[i],points[i+1]));

                if (addCenter)
                {
                    i += 3;
                }
                addCenter = !addCenter;

            }

            edges.Add(new Edge(points[edgeNumber-1], points[0]));


            uv = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                uv[i] = points[i].Position.v2;
            }

        }
    }
}