using System.Collections.Generic;

namespace MapCore.Models
{
	public class PropScriptInfo
	{
		public int PropId { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public short ModelId { get; set; }
		public List<string> Scripts { get; set; } = new List<string>();
	}
}
