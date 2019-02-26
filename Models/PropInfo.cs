#pragma warning disable CS1591

namespace MapCore.Models
{
	public class PropInfo
	{
		public uint Id { get; set; }
		public string Category { get; set; }
		public string PropName { get; set; }
		public string LightMapType { get; set; }
		public string RenderType { get; set; }
		public string VisibleRatio { get; set; }
	}
}
