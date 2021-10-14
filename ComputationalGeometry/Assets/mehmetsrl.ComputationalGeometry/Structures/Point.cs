using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures
{
    [System.Serializable]
    public class Point: ICloneable
    {
        #region Accesors
        [SerializeField]
        public float this[int i]
        {
            get
            {
                return position[i];
            }
            set
            {
                position[i] = value;
            }
        }

        public float x
        {
            get
            {
                if (position.dimension > 0)
                    return position[0];
                return default;
            }
            set
            {
                if (position.dimension > 0)
                    position[0] = value;
            }
        }

        public float y
        {
            get
            {
                if (position.dimension > 1)
                    return position[1];
                return default;
            }
            set
            {
                if (position.dimension > 1)
                    position[1] = value;
            }
        }

        public float z
        {
            get
            {
                if (position.dimension > 2)
                    return position[2];
                return default;
            }
            set
            {
                if (position.dimension > 2)
                    position[2] = value;
            }
        }

        #endregion
        [SerializeField]
        string description;

        public Vector Position
        {
            get { return position; }
        }

        [SerializeField]
        private Vector position;

        public LinkedList<Edge> conectedEdges;

        public Point(Vector position)
        {
            this.position = position;
            description = Position.ToString();
            conectedEdges = new LinkedList<Edge>();
        }

        public void LinkToEdge(Edge e)
        {
            conectedEdges.AddLast(e);
        }

        public override string ToString()
        {
            return ("Point at: " + Position);
        }

        public List<Point> GetNeigbourPoints()
        {
            List<Point> neigbours = new List<Point>(conectedEdges.Count);

            foreach (var edge in conectedEdges)
            {
                if (edge.StartPoint == edge.EndPoint)
                {
                    Debug.LogError("There is an error at start and end point");
                    continue;
                }
                if (edge.StartPoint == this)
                {
                    neigbours.Add(edge.EndPoint);
                }
                else
                {
                    neigbours.Add(edge.StartPoint);
                }
            }

            return neigbours;
        }

        public override bool Equals(object obj)
        {
            if(obj as Point !=null)
                return this == obj as Point;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            return new Point(new Vector(this.position.RawData));
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Position.Equals(p2.Position);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1==p2);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.Position - p2.Position);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.Position + p2.Position);
        }

        public static float operator *(Point p1, Point p2)
        {

            return p1.Position * p2.Position;
        }

        public static Point operator *(Point p, float multiplyer)
        {
            return new Point(p.Position*multiplyer);
        }

        public static Point operator *(float multiplyer, Point p)
        {
            return p*multiplyer;
        }
        public static Point operator /(Point p, float devider)
        {
            return new Point(p.Position / devider);
        }
    }
}