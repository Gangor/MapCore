#pragma warning disable CS1591

namespace MapCore.Models
{
	public class RectangleVector
	{
		public Vector LeftTop { get; set; } = new Vector();
		public Vector RightBottom { get; set; } = new Vector();

		public Vector Center { get; set; } = new Vector();

		public RectangleVector Clone()
		{
			return (RectangleVector)MemberwiseClone();
		}
	}
}
