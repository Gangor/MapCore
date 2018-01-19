namespace MapCore.Models
{
	public class KSegment
	{
		public uint Version { get; set; }
		public ushort[] Tile { get; set; } = new ushort[3];
		public KVertex[,] HsVector { get; set; } = new KVertex[6, 6];
	}
}
