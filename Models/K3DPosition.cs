namespace MapCore.Models
{
    public class K3DPosition
	{
		private int id;
		private static int NextId;

		public float X { get; set; }
		public float Y { get; set; }
        public float Z { get; set; }
		public int Id { get { return id; } }
		
		static K3DPosition() { NextId = -1; }
		public K3DPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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

			K3DPosition point = (K3DPosition)obj;

			if (X != point.X)
				return false;

			if (Y != point.Y)
				return false;

			return Z == point.Z;
		}

		public static bool operator ==(K3DPosition left, K3DPosition right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(K3DPosition left, K3DPosition right)
		{
			return !Equals(left, right);
		}

		public static bool operator >(K3DPosition left, K3DPosition right)
		{
			return ((left.X == right.X && left.Y > right.Y)
				|| left.X > right.X && left.Y > right.Y);
		}

		public static bool operator <(K3DPosition left, K3DPosition right)
		{
			return ((left.X == right.X && left.Y < right.Y)
				|| left.X < right.X && left.Y < right.Y);
		}
	}
}
