#pragma warning disable CS1591

namespace MapCore.Models
{
	public class TerrainLight
	{
		public KColor Diffuse { get; set; } = new KColor();
		public KColor Specular { get; set; } = new KColor();
		public KColor Ambient { get; set; } = new KColor();
		public float Attenuation0 { get; set; }
		public float Attenuation1 { get; set; }
		public float Attenuation2 { get; set; }
	}
}
