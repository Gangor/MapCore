using System.Collections.Generic;
using System.Drawing;

namespace MapCore.Models
{
	public class LocationInfo
	{		
		public int Priority { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float Radius { get; set; }
		public string Description { get; set; }
		public string Scripts { get; set; }
		public List<Polygon2> Polygons { get; set; } = new List<Polygon2>();
	}
}
