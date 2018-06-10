#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class EventArea
	{
		public int AreaId { get; set; }
		public List<Polygon> Polygons { get; set; } = new List<Polygon>();
	}
}
