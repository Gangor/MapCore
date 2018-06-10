#pragma warning disable CS1591

using System.Collections.Generic;
using System.Drawing;

namespace MapCore.Models
{
	public class Polygon : List<Vector>
	{
		public static implicit operator Polygon (Point[] points)
		{
			var polygon = new Polygon();

			for (int i = 0; i < points.Length; i++)
			{
				polygon.Add((Vector)points[i]);
			}

			return polygon;
		}

		public static implicit operator Polygon(PointF[] points)
		{
			var polygon = new Polygon();

			for (int i = 0; i < points.Length; i++)
			{
				polygon.Add((Vector)points[i]);
			}

			return polygon;
		}

		public static implicit operator Point[] (Polygon polygon)
		{
			var points = new Point[polygon.Count];

			for (int i = 0; i < polygon.Count; i++)
			{
				points[i] = (Point)polygon[i];
			}

			return points;
		}

		public static implicit operator PointF[] (Polygon polygon)
		{
			var points = new PointF[polygon.Count];

			for (int i = 0; i < polygon.Count; i++)
			{
				points[i] = (PointF)polygon[i];
			}

			return points;
		}
	}
}
