#pragma warning disable CS1591

namespace MapCore.Models
{
    public class TerrainProperties
    {
		public TerrainLight Primary { get; set; } = new TerrainLight();
		public TerrainLight Secondary { get; set; } = new TerrainLight();
		public KColor Sky { get; set; } = new KColor();
        public KColor Fog { get; set; } = new KColor();
		public float FogNear { get; set; } = 0.1f;
        public float FogFar { get; set; } = 5500.0f;
		public uint SkyType { get; set; } = 0;
		public bool ShowTerrainInGame { get; set; } = true;
    }
}
