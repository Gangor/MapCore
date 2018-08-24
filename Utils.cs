using MapCore.Models;
using System.Collections.Generic;
using System.Drawing;

namespace MapCore
{
	/// <summary>
	/// Utility for help manage 2D/3D coordonate
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Get the center point list of the polygon
		/// </summary>
		/// <returns>Center point of the point list</returns>
		/// <returns></returns>
		public static Vector GetCenterPolygon(List<Vector> points) { return GetCenterPolygon(points.ToArray()); }

		/// <summary>
		/// Get the center point of the polygon
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns>Center point of the polygon</returns>
		public static Vector GetCenterPolygon(Polygon polygon) { return GetCenterPolygon(polygon.ToArray()); }

		/// <summary>
		/// Get the center point of the point array
		/// </summary>
		/// <param name="points">Coordonate point</param>
		/// <returns>Center point of the polygon</returns>
		public static Vector GetCenterPolygon(Vector[] points)
		{
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

			var area = GetPolygonArea(points);

			centerX /= (6 * area);
			centerY /= (6 * area);

			if (centerX < 0)
			{
				centerX = -centerX;
				centerY = -centerY;
			}
			return new Vector(centerX, centerY);
		}

		/// <summary>
		/// Get area of the polygon
		/// </summary>
		/// <param name="points">Coordonate point</param>
		/// <returns>Total surface</returns>
		public static float GetPolygonArea(Vector[] points)
		{
			int len = points.Length;

			float area = 0;
			for (int i = 0; i < len; i++)
			{
				area += (points[(i + 1) % len].X - points[i].X) * (points[(i + 1) % len].Y + points[i].Y) / 2;
			}

			return area;
		}
	}
}
