using System.Collections.Generic;

namespace MapCore.Models
{
	public class Location
	{
		public int Left { get; set; }
		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
		public string Description { get; set; }
		public List<string> Scripts { get; set; } = new List<string>();
	}
}
