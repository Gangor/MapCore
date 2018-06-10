#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Grass
	{
		public int SegmentId { get; set; }
		public int GrassId { get; set; }
		public List<GrassProp> Props { get; set; } = new List<GrassProp>();
	}
}
