#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class NpcProp
	{
		public int PropId { get; set; }
		public Vector Position { get; set; } = new Vector();
		public short ModelId { get; set; }
		public List<string> Scripts { get; set; } = new List<string>();
	}
}
