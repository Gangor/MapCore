using System.Collections.Generic;

namespace MapCore.Models
{
	public class KGrass
	{
		public int SegmentId { get; set; }
		public int GrassId { get; set; }
		public List<KGrassProp> Props { get; set; } = new List<KGrassProp>();

		public KGrass Clone()
		{
			return (KGrass)MemberwiseClone();
		}
	}
}
