#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Respawn
	{
		public RectangleVector Rectangle { get; set; } = new RectangleVector();
		public string Description { get; set; }
		public List<string> Scripts { get; set; } = new List<string>();
	}
}
