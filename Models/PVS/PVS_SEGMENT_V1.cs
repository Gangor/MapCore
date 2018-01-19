using System.Collections.Generic;

namespace MapCore.Models
{
	public class PVS_SEGMENT_V1
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
		public List<SEGMENT_DATA_V1> IncludeSegments { get; set; } = new List<SEGMENT_DATA_V1>();
	}

	public class SEGMENT_DATA_V1
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
	}
}
