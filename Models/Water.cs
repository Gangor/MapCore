#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Water
	{
		public int WaterId { get; set; }
		public KColor Color { get; set; } = new KColor();
		public Vector[] Points { get; set; } = new Vector[3];
		public int UseReflect { get; set; }
	}
}
