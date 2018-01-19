using MapCore.Enum;
using System.Drawing;

namespace MapCore.Models
{
	public class StructLights
	{
		public K3DPosition Position { get; set; } = new K3DPosition(0f, 0f, 0f);
		public float Height { get; set; }
		public K3DVector Direction { get; set; } = new K3DVector();
		public KColor Specular { get; set; } = new KColor();
		public KColor Diffuse { get; set; } = new KColor();
		public KColor Ambient { get; set; } = new KColor();
		public LightsType LightType { get; set; } = LightsType.LIGHT_NONE;
	}
}
