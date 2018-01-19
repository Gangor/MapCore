namespace MapCore.Models
{
	public class K2DPosition
	{
		private int id;
		private static int NextId;

		public int X { get; set; }
		public int Y { get; set; }
		public int Id { get { return id; } }

		static K2DPosition() { NextId = -1; }
		public K2DPosition(int x, int y)
		{
			X = x;
			Y = y;
			id = ++(NextId);
		}

		public override int GetHashCode()
		{
			return id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			K2DPosition point = (K2DPosition)obj;

			if (X != point.X)
				return false;

			return Y == point.Y;
		}

		public static bool operator ==(K2DPosition left, K2DPosition right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(K2DPosition left, K2DPosition right)
		{
			return !Equals(left, right);
		}

		public static bool operator >(K2DPosition left, K2DPosition right)
		{
			return (left.X != right.X || left.Y != right.Y);
		}

		public static bool operator <(K2DPosition left, K2DPosition right)
		{
			return ((left.X == right.X && left.Y > right.Y) 
				|| left.X > right.X && left.Y > right.Y);
		}
	}
}
