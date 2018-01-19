namespace MapCore.Models
{
    public class KProperties
    {
		public K3DLight Primary { get; set; } = new K3DLight();
		public K3DLight Secondary { get; set; } = new K3DLight();
		public KColor Sky { get; set; } = new KColor();
        public KColor Fog { get; set; } = new KColor();
		public float FogNear { get; set; } = 0.1f;
        public float FogFar { get; set; } = 5500.0f;
		public uint SkyType { get; set; } = 0;
		public bool ShowTerrainInGame { get; set; } = true;
    }
}
