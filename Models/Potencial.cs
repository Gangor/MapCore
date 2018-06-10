#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class PotencialSegment
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
		public List<PotencialSegmentData> IncludeSegments { get; set; } = new List<PotencialSegmentData>();
	}

	public class PotencialProp
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
		public List<PotencialPropData> IncludeProps { get; set; } = new List<PotencialPropData>();
	}

	public class PotencialPropData
	{
		public uint SegmentIdx { get; set; }
		public uint PropIdx { get; set; }
	}

	public class PotencialSegmentData
	{
		public byte SegmentX { get; set; }
		public byte SegmentY { get; set; }
	}
}
