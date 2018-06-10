#pragma warning disable CS1591

using System.Drawing;

namespace MapCore.Models
{
	public class KColor
	{
		public Color Color { get; set; }
		
		public KColor(byte b = 255, byte g = 255, byte r = 255, byte a = 255)
		{
			Color = Color.FromArgb(a, r, g, b);
		}
	}
}
