#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Region
	{		
		public int Priority { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float Radius { get; set; }
		public string Description { get; set; }
		public string Scripts { get; set; }
		public List<Polygon> Polygons { get; set; } = new List<Polygon>();
	}
}
