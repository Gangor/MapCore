#pragma warning disable CS1591

using System;
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
		/// Create a new vector empty
		/// </summary>
		public static Vector Empty => new Vector();

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
			hashCode = hashCode * -1521134295 + Z.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Get real coordonate
		/// Be carefull, dont erase vector with this coordonnate
		/// </summary>
		public virtual Vector Clone()
		{
			return new Vector(X, Y, Z);
		}

		/// <summary>
		/// Get game coordonate
		/// </summary>
		public virtual Vector GetGameCoordonate(int x, int y)
		{
			var vector = Clone();

			vector.X = vector.X + Global.TerrainLenght * x;
			vector.Y = vector.Y + Global.TerrainLenght * y;

			return vector;
		}

		/// <summary>
		/// Get current game X
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public virtual float GetGameX(int x)
		{
			return X + Global.TerrainLenght * x;
		}

		/// <summary>
		/// Get current game Y
		/// </summary>
		/// <param name="y"></param>
		/// <returns></returns>
		public virtual float GetGameY(int y)
		{
			return Y + Global.TerrainLenght * y;
		}

		/// <summary>
		/// Get segment coordonate
		/// </summary>
		public virtual Vector GetSegmentCoordonate()
		{
			var vector = Clone();

			vector.X = vector.X % (Global.TileLenght * Global.TileCountPerSegment);
			vector.Y = vector.Y % (Global.TileLenght * Global.TileCountPerSegment);

			return vector;
		}

		/// <summary>
		/// Get current segment X
		/// </summary>
		/// <returns></returns>
		public virtual int GetSegmentX()
		{
			return Math.Min((int)(X / (Global.TileLenght * Global.TileCountPerSegment)), Global.SegmentCountPerMap - 1);
		}

		/// <summary>
		/// Get current segment Y
		/// </summary>
		/// <returns></returns>
		public virtual int GetSegmentY()
		{
			return Math.Min((int)(Y / (Global.TileLenght * Global.TileCountPerSegment)), Global.SegmentCountPerMap - 1);
		}

		/// <summary>
		/// Get segment id
		/// </summary>
		/// <returns></returns>
		public virtual int GetSegmentId()
		{
			return GetSegmentX() + GetSegmentY() * Global.SegmentCountPerMap;
		}

		/// <summary>
		/// Get real coordonate
		/// Be carefull, dont erase vector with this coordonnate
		/// </summary>
		public virtual Vector GetTileCoordonate()
		{
			var vector = Clone();

			vector.X = vector.X % (Global.TileLenght);
			vector.Y = vector.Y % (Global.TileLenght);

			return vector;
		}

		/// <summary>
		/// Get current tile X
		/// </summary>
		/// <returns></returns>
		public virtual int GetTileX()
		{
			return Math.Min((int)(X % (Global.TileLenght * Global.TileCountPerSegment) / Global.TileLenght), Global.TileCountPerSegment - 1);
		}

		/// <summary>
		/// Get current tile Y
		/// </summary>
		/// <returns></returns>
		public virtual int GetTileY()
		{
			return Math.Min((int)(Y % (Global.TileLenght * Global.TileCountPerSegment) / Global.TileLenght), Global.TileCountPerSegment - 1);
		}

		/// <summary>
		/// Get tile id
		/// </summary>
		/// <returns></returns>
		public virtual int GetTileId()
		{
			return GetTileX() + GetTileY() * Global.TileCountPerSegment;
		}

		/// <summary>
		/// Get point with rotate 180 and flip Y
		/// </summary>
		/// <returns>Coordonate point</returns>
		public Vector Rotate180FlipY()
		{
			var vector = Clone();

			vector.X = Global.TerrainLenght - vector.X;
			vector.Y = Global.TerrainLenght - vector.Y;
			vector.X = Global.TerrainLenght - vector.X;

			return vector;
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
			return new Vector
			{
				X = obj.X,
				Y = obj.Y
			};
		}

		public static explicit operator Vector(PointF point)
		{
			return new Vector
			{
				X = point.X,
				Y = point.Y
			};
		}

		public static explicit operator Point(Vector vector)
		{
			return new Point
			{
				X = (int)vector.X,
				Y = (int)vector.Y
			};
		}

		public static explicit operator PointF(Vector point)
		{
			return new PointF
			{
				X = point.X,
				Y = point.Y
			};
		}
	}
}
