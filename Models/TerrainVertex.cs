#pragma warning disable CS1591

namespace MapCore.Models
{
	public class TerrainVertex
	{		
		public float Height { get; set; }
		public uint[] FillBits { get; set; } = new uint[2];
		public long Attribute { get; set; }
		public KColor Color { get; set; } = new KColor();
	}
}
