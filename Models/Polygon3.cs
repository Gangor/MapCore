using System.Collections.Generic;
using System.Drawing;

namespace MapCore.Models
{
	public class Polygon3 : List<K3DPosition>
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
		/// <returns>Polygon 3D</returns>
		public static Polygon3 FromPoints(PointF[] points)
		{
			var polygon = new Polygon3();
			foreach (var point in points)
				polygon.Add(new K3DPosition(point.X, point.Y, 0f));

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
