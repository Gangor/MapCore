#pragma warning disable CS1591

namespace MapCore.Models
{
	public class GrassProp
	{
		public int GrassId { get; set; }
		public Vector Position { get; set; } = new Vector();
		public float RotateX { get; set; }
		public float RotateY { get; set; }
		public float RotateZ { get; set; }
	}
}
