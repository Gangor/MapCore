#pragma warning disable CS1591

namespace MapCore.Models
{
	public class Light
	{
		public Vector Position { get; set; } = new Vector();
		public float Height { get; set; }
		public Vector Direction { get; set; } = new Vector();
		public KColor Specular { get; set; } = new KColor();
		public KColor Diffuse { get; set; } = new KColor();
		public KColor Ambient { get; set; } = new KColor();
		public LightsType LightType { get; set; } = LightsType.LIGHT_NONE;
	}

	public enum LightsType : int
	{
		LIGHT_NONE = 0x0,
		LIGHT_SPOT = 0x1,
		LIGHT_DIRECT = 0x2,
		LIGHT_OMNI = 0x3,
	};
}
