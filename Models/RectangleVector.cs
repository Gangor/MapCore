#pragma warning disable CS1591

using System;

namespace MapCore.Models
{
	public class RectangleVector
	{
		public Vector LeftTop { get; set; } = new Vector();
		public Vector RightBottom { get; set; } = new Vector();
		public Vector Center { get; set; } = new Vector();

		public Vector GetCenterPoint()
		{
			var centerX = (Math.Max(LeftTop.X, RightBottom.X) - Math.Min(LeftTop.X, RightBottom.X)) / 2 + Math.Min(LeftTop.X, RightBottom.X);
			var centerY = (Math.Max(LeftTop.Y, RightBottom.Y) - Math.Min(LeftTop.Y, RightBottom.Y)) / 2 + Math.Min(LeftTop.Y, RightBottom.Y);
			return new Vector(centerX, centerY);
		}

		public RectangleVector Clone()
		{
			return (RectangleVector)MemberwiseClone();
		}
	}
}
