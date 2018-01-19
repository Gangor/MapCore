using System.Collections.Generic;

namespace MapCore.Models
{
    public class Water
    {
		public K3DPosition[] Points { get; set; } = new K3DPosition[3];
		public int UseReflect { get; set; }
		public int WaterId { get; set; }
		public KColor Color { get; set; } = new KColor();
	}
}
