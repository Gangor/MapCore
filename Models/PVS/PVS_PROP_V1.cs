using System.Collections.Generic;

namespace MapCore.Models
{
	public class PVS_PROP_V1
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
		public List<PROP_DATA_V1> IncludeProps { get; set; } = new List<PROP_DATA_V1>();
	}

	public class PROP_DATA_V1
	{
		public uint SegmentIdx { get; set; }
		public uint PropIdx { get; set; }
	}
}
