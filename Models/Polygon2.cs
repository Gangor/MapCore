using System.Collections.Generic;
using System.Drawing;

namespace MapCore.Models
{
	public class Polygon2 : List<K2DPosition>
	{
		/// <summary>
		/// Create array of the points
		/// </summary>
		/// <returns></returns>
		public PointF[] ToPointArray()
		{
			var points = new PointF[Count];
			for (var i = 0; i < Count; i++)
				points[i] = new PointF(this[i].X, this[i].Y);

			return points;
		}

		/// <summary>
		/// Create polygon from the points
		/// </summary>
		/// <param name="points">Coordonate point</param>
		/// <returns>Polygon 2D</returns>
		public static Polygon2 FromPoints(PointF[] points)
		{
			var polygon = new Polygon2();
			foreach (var point in points)
				polygon.Add(new K2DPosition((int)point.X, (int)point.Y));

			return polygon;
		}

		/// <summary>
		/// Create enumerate of the points
		/// </summary>
		/// <returns></returns>
		public List<PointF> ToList()
		{
			var points = new List<PointF>();
			for (var i = 0; i < Count; i++)
				points.Add(new PointF(this[i].X, this[i].Y));

			return points;
		}
	}
}
