using System.Collections.Generic;

namespace MapCore.Models
{
	public class RecordNfp
	{
		public int Id { get; set; }
		public List<Polygon3> Polygons { get; set; } = new List<Polygon3>();
		public string Description { get; set; }
	}
}
