#pragma warning disable CS1591

namespace MapCore.Models
{
	public class TerrainSegment
	{
		public uint Version { get; set; }
		public ushort[] Tile { get; set; } = new ushort[3];
		public TerrainVertex[,] HsVector { get; set; } = new TerrainVertex[6, 6];
	}
}