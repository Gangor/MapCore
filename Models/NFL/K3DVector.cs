namespace MapCore.Models
{
	public class K3DVector
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public K3DVector() { }
		public K3DVector(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}
