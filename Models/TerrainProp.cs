#pragma warning disable CS1591

namespace MapCore.Models
{
	public class TerrainProp
	{
		public int SegmentId { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float RotateX { get; set; }
		public float RotateY { get; set; }
		public float RotateZ { get; set; }
		public float ScaleX { get; set; }
		public float ScaleY { get; set; }
		public float ScaleZ { get; set; }
		public ushort PropNum { get; set; }
		public bool HeightLocked { get; set; }
		public float LockHeight { get; set; }
		public short TextureGroup { get; set; }
	}
}
