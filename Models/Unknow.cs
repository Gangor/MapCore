#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Unknow
	{
		public int Id { get; set; }
		public List<Polygon> Polygons { get; set; } = new List<Polygon>();
		public string Description { get; set; }
	}
}
