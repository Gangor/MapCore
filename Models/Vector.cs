#pragma warning disable CS1591

using System.Drawing;

namespace MapCore.Models
{
	/// <summary>
	/// Model for position
	/// </summary>
	public class Vector
	{
		/// <summary>
		/// Get or set axe X
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Get or set axe Y
		/// </summary>
		public float Y { get; set; }

		/// <summary>
		/// Get or set axe Z
		/// </summary>
		public float Z { get; set; }

		/// <summary>
		/// Construct a new point
		/// </summary>
		public Vector() { }
		public Vector(int x, int y) : this(x, y, 0) { }
		public Vector(float x, float y) : this(x, y, 0) { }
		public Vector(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			Vector point = (Vector)obj;

			if (X != point.X)
				return false;

			if (Y != point.Y)
				return false;

			return Z == point.Z;
		}

		public override int GetHashCode()
		{
			var hashCode = 1166230731;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(Vector left, Vector right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Vector left, Vector right)
		{
			return !Equals(left, right);
		}

		public static bool operator >(Vector left, Vector right)
		{
			return ((left.X == right.X && left.Y > right.Y)
				|| left.X > right.X && left.Y > right.Y);
		}

		public static bool operator >=(Vector left, Vector right)
		{
			return ((left.X == right.X && left.Y > right.Y)
				|| left.X > right.X && left.Y > right.Y);
		}

		public static bool operator <(Vector left, Vector right)
		{
			return ((left.X == right.X && left.Y <= right.Y)
				|| left.X > right.X && left.Y <= right.Y);
		}

		public static bool operator <=(Vector left, Vector right)
		{
			return ((left.X == right.X && left.Y <= right.Y)
				|| left.X > right.X && left.Y <= right.Y);
		}

		public static Vector operator ++(Vector obj)
		{
			obj.Y += 1;
			return obj;
		}

		public static Vector operator --(Vector obj)
		{
			obj.Y -= 1;
			return obj;
		}

		public static explicit operator Vector(Point obj)
		{
			var position = new Vector();
			position.X = obj.X;
			position.Y = obj.Y;

			return position;
		}

		public static explicit operator Vector(PointF obj)
		{
			var position = new Vector();
			position.X = obj.X;
			position.Y = obj.Y;

			return position;
		}

		public static explicit operator Point(Vector obj)
		{
			var position = new Point();
			position.X = (int)obj.X;
			position.Y = (int)obj.Y;

			return position;
		}

		public static explicit operator PointF(Vector obj)
		{
			var position = new PointF();
			position.X = obj.X;
			position.Y = obj.Y;

			return position;
		}
	}
}
