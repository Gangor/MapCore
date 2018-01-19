using System.Collections.Generic;

namespace MapCore.Models
{
	public class EventAreaScript
	{
		public int AreaId { get; set; }
		public List<Polygon2> Polygons { get; set; } = new List<Polygon2>();
	}
}
