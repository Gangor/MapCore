#pragma warning disable CS1591

namespace MapCore.Models
{
	public class QuestProp
	{		
		public int QuestPropID { get; set; }
		public ushort PropNum { get; set; }
		public Vector Position { get; set; } = new Vector();
		public float OffSet { get; set; }
		public float RotateX { get; set; }
		public float RotateY { get; set; }
		public float RotateZ { get; set; }
		public float ScaleX { get; set; }
		public float ScaleY { get; set; }
		public float ScaleZ { get; set; }
		public bool LockedHeight { get; set; }
		public float LockHeight { get; set; }
		public short TextureGroupIndex { get; set; }
	}
}
