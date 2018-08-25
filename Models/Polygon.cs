#pragma warning disable CS1591

using System.Collections.Generic;
using System.Drawing;

namespace MapCore.Models
{
	public class Polygon : List<Vector>
	{
		/// <summary>
		/// Get the center point of the point array
		/// </summary>
		/// <returns>Center point of the polygon</returns>
		public Vector GetCenterPoint()
		{
			var points = ToArray();
			var len = points.Length;
			var pts = new PointF[len + 1];

			points.CopyTo(pts, 0);
			pts[len] = pts[0];

			float centerX = 0;
			float centerY = 0;
			float secondFactor = 0;

			for (int i = 0; i < len; i++)
			{
				secondFactor = pts[i].X * pts[i + 1].Y - pts[i + 1].X * pts[i].Y;
				centerX += (pts[i].X + pts[i + 1].X) * secondFactor;
				centerY += (pts[i].Y + pts[i + 1].Y) * secondFactor;
			}

			float area = 0;
			for (int i = 0; i < len; i++)
			{
				area += (points[(i + 1) % len].X - points[i].X) * (points[(i + 1) % len].Y + points[i].Y) / 2;
			}

			centerX /= (6 * area);
			centerY /= (6 * area);

			if (centerX < 0)
			{
				centerX = -centerX;
				centerY = -centerY;
			}
			return new Vector(centerX, centerY);
		}

		#region Operator

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

		#endregion
	}
}
