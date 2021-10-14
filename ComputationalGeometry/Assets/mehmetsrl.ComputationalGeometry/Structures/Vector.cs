using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.ComputationalGeometry.Structures
{
	[System.Serializable]
    public class Vector : IEnumerable, IComparable,ICloneable
    {
        #region Properties
		[SerializeField]
        float[] data;
		[SerializeField]
		string desc;
		#endregion

		#region Constructors
		public Vector(int dim)
		{
			data = new float[dim];
		}

		public Vector2 v2
        {
            get { return new Vector2(data[0],data[1]); }
        }
		public Vector3 v3
		{
			get { return new Vector3(data[0], data[1],data[1]); }
		}

		public Vector(Vector2 v2)
		{
			data = new float[2];
			data[0] = v2.x;
			data[1] = v2.y;
		}
		public Vector(Vector3 v3)
		{
			data = new float[3];
			data[0] = v3.x;
			data[1] = v3.y;
			data[2] = v3.z;
		}

		public Vector(params float[] data)
		{
			this.data = new float[data.Length];
			data.CopyTo(this.data, 0);
		}
		public Vector(Vector v) : this(v.data) { }
		#endregion


		#region Accesors
		#region specialConstructors
		public static Vector invalid
		{
			get { return new Vector(0); }
		}
		public static Vector zero2D
		{
			get { return new Vector(Vector2.zero); }
		}
		public static Vector right2D
		{
			get { return new Vector(Vector2.right); }
		}
		public static Vector left2D
		{
			get { return new Vector(Vector2.left); }
		}
		public static Vector up2D
		{
			get { return new Vector(Vector2.up); }
		}
		public static Vector down2D
		{
			get { return new Vector(Vector2.down); }
		}

		public static Vector zero3D
		{
			get { return new Vector(Vector3.zero); }
		}
		public static Vector right3D
		{
			get { return new Vector(Vector3.right); }
		}
		public static Vector left3D
		{
			get { return new Vector(Vector3.left); }
		}
		public static Vector up3D
		{
			get { return new Vector(Vector3.up); }
		}
		public static Vector down3D
		{
			get { return new Vector(Vector3.down); }
		}
		public static Vector forward3D
		{
			get { return new Vector(Vector3.forward); }
		}
		public static Vector back3D
		{
			get { return new Vector(Vector3.back); }
		}
		#endregion

		public float[] RawData
		{
			get
			{
				return data;
			}
		}

		public float this[int i]
		{
			get
			{
				return data[i];
			}
			set
			{
				data[i] = value;
			}
		}

		public int dimension
		{
			get
			{
				return data.Length;
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return this * this;
			}
		}

		public float magnitude
		{
			get
			{
				return Mathf.Sqrt(sqrMagnitude);
			}
		}
		public float elementSum
		{
			get
			{
				int i;
				float result = 0;
				for (i = 0; i < dimension; i++)
					result += data[i];
				return result;
			}
		}
		#endregion

		#region InterfaceImplementations

		public int CompareTo(object v)
        {
			Vector other = v as Vector;
			if (this == null || other == null)
				return 0;
			float magnitude, otherMagnitude;
			magnitude = this.magnitude;
			otherMagnitude = other.magnitude;
			if (magnitude > otherMagnitude)
				return 1;
			if (magnitude < otherMagnitude)
				return -1;
			int i;
			for (i = 0; i < this.dimension; i++)
			{
				if (this[i] > other[i])
					return 1;
				if (this[i] < other[i])
					return -1;
			}
			return 0;
		}

        public IEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }
		#endregion

		#region UseCaseMethods

		public override int GetHashCode()
		{
			int result = 0;
			foreach (float val in data)
				result = result ^ val.GetHashCode();
			return result;
		}

		System.Text.StringBuilder builder = new System.Text.StringBuilder();

		public override string ToString()
		{
			builder.Clear();
			builder.Append("(");
			int i;
			for (i = 0; i < data.Length; i++)
			{
				builder.Append(data[i].ToString("G4"));
				if (i < data.Length - 1)
					builder.Append(";");
			}
			builder.Append(")");
			return builder.ToString();
		}

		public void Randomize(float min, float max)
		{
			int i;
			for (i = 0; i < data.Length; i++)
			{
				this[i] = min + UnityEngine.Random.Range(min,max);
			}
		}

		public override bool Equals(object v)
		{
			Vector other = v as Vector;
			if (other == null || data.Length != other.data.Length)
				return false;
			int i;
			for (i = 0; i < data.Length; i++)
			{
				if (Mathf.Abs(data[i] - other.data[i]) > float.Epsilon)
					return false;
			}
			return true;
		}

		public void Multiply(float r)
		{
			int i;
			for (i = 0; i < data.Length; i++)
			{
				this[i] *= r;
			}
		}

		public void Add(Vector v)
		{
			int i;
			for (i = 0; i < data.Length; i++)
			{
				this[i] += v[i];
			}
		}

		public void Add(float d)
		{
			int i;
			for (i = 0; i < data.Length; i++)
			{
				this[i] += d;
			}
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			if (v1.dimension != v2.dimension)
				throw new Exception("Vectors of different dimension!");
			Vector result = new Vector(v1.dimension);
			int i;
			for (i = 0; i < v1.dimension; i++)
				result[i] = v1[i] - v2[i];
			return result;
		}

		public static Vector operator +(Vector v1, Vector v2)
		{
			if (v1.dimension != v2.dimension)
				throw new Exception("Vectors of different dimension!");
			Vector result = new Vector(v1.dimension);
			int i;
			for (i = 0; i < v1.dimension; i++)
				result[i] = v1[i] + v2[i];
			return result;
		}

		public static float operator *(Vector v1, Vector v2)
		{
			if (v1.dimension != v2.dimension)
				throw new Exception("Vectors of different dimension!");
			float result = 0;
			int i;
			for (i = 0; i < v1.dimension; i++)
				result += v1[i] * v2[i];
			return result;
		}

		public static Vector operator *(Vector v, float multiplyer)
		{
			Vector result = new Vector(v.dimension);
			int i;
			for (i = 0; i < v.dimension; i++)
				result[i] = v[i] * multiplyer;
			return result;
		}

		public static Vector operator *(float multiplyer, Vector v)
		{
			return v * multiplyer;
		}

		public static Vector operator /(Vector v, float devider)
		{
			Vector result = new Vector(v.dimension);
			int i;
			for (i = 0; i < v.dimension; i++)
				result[i] = v[i] / devider;
			return result;
		}

		public static explicit operator float[](Vector v)
		{
			return v.data;
		}
		
		public static float Distance(Vector v1, Vector v2)
		{
			if (v1.dimension != v2.dimension)
				return -1;
			int i;
			float result = 0, dist;
			for (i = 0; i < v1.dimension; i++)
			{
				dist = (v1[i] - v2[i]);
				result += dist * dist;
			}
			return Mathf.Sqrt(result);

		}


		public virtual Vector Clone()
		{
			return new Vector(data);
		}

        object ICloneable.Clone()
        {
			return new Vector(data);
		}
        #endregion
    }

}