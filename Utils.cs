using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapCore
{
	public static class Utils
	{
		public const int tileLenght = 42;

		/// <summary>
		/// Convertion coordonate to the point
		/// </summary>
		/// <param name="map">Map location</param>
		/// <param name="point">Coordonate point</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="divide">Need multiplication quotient</param>
		/// <param name="multiplicate">Need to divide tile size</param>
		/// <returns>Game or Map point</returns>
		public static PointF ConvertCoordonateToPoint(Point? map, Point point, float zoom, bool divide, bool multiplicate)
		{
			var position = ConvertCoordonateToPoint(map, new PointF(point.X, point.Y), zoom, divide, multiplicate);
			return new Point((int)position.X, (int)position.Y);
		}

		/// <summary>
		/// Convertion coordonate to the point
		/// </summary>
		/// <param name="point"></param>
		/// <param name="zoom"></param>
		/// <param name="imprecision"></param>
		/// <param name="tile"></param>
		/// <returns>Game or Map point</returns>
		public static PointF ConvertCoordonateToPoint(Point? map, PointF point, float zoom, bool divide, bool multiplicate)
		{
			var position = GetPointRotate180FlipY(new PointF((point.X / zoom), (point.Y / zoom)));
			if (map.HasValue)
			{
				position.X -= map.Value.X * 16128;
				position.Y -= map.Value.Y * 16128;
			}
			if (multiplicate)
			{
				position.X *= tileLenght;
				position.Y *= tileLenght;
			}
			if (divide)
			{
				position.X /= 8;
				position.Y /= 8;
			}

			position.X /= 7.875f;
			position.Y /= 7.875f;

			return position;
		}

		/// <summary>
		/// Convertion coordonate to the segment point
		/// </summary>
		/// <param name="map">Map location</param>
		/// <param name="point">Coordonate point</param>
		/// <returns>
		/// Segment number
		/// Segment point
		/// </returns>
		public static (int, PointF) ConvertCoordonateToSegmentPoint(Point? map, PointF point)
		{
			var pt = new PointF(point.X, point.Y);

			if (map.HasValue)
			{
				pt.X -= map.Value.X * 16128;
				pt.Y -= map.Value.Y * 16128;
			}

			int segmentX = Math.Min(((int)pt.X / tileLenght / 6), 63);
			int segmentY = Math.Min(((int)pt.Y / tileLenght / 6), 63);
			int segmentNumber = segmentX + segmentY * 64;

			pt.X = segmentNumber % 64 * tileLenght * 6;
			pt.Y = segmentNumber / 64 * tileLenght * 6;

			return (segmentNumber, pt);
		}

		/// <summary>
		/// Convertion coordonates to the points
		/// </summary>
		/// <param name="map"></param>
		/// <param name="points"></param>
		/// <param name="zoom"></param>
		/// <param name="divide"></param>
		/// <param name="multiplicate"></param>
		/// <returns></returns>
		public static PointF[] ConvertCoordonatesToPoints(Point? map, PointF[] points, float zoom, bool divide, bool multiplicate)
		{
			var data = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
				data[i] = ConvertCoordonateToPoint(map, points[i], zoom, divide, multiplicate);

			return data;
		}

		/// <summary>
		/// Convertion point to the coordonate
		/// </summary>
		/// <param name="map">Map location</param>
		/// <param name="point">Map coordonate point</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="multiplicate">Need multiplication quotient</param>
		/// <param name="divide">Need to divide tile size</param>
		/// <returns>Map or game coordonate point</returns>
		public static Point ConvertPointToCoordonate(Point? map, Point point, float zoom, bool multiplicate, bool divide)
		{
			var position = ConvertPointToCoordonate(map, new PointF(point.X, point.Y), zoom, multiplicate, divide);
			return new Point((int)position.X, (int)position.Y);
		}

		/// <summary>
		/// Convertion point to the coordonate
		/// </summary>
		/// <param name="map">Map location</param>
		/// <param name="point">Map coordonate</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="multiplicate">Need multiplication quotient</param>
		/// <param name="divide">Need to divide tile size</param>
		/// <returns>Map or game coordonate point</returns>
		public static PointF ConvertPointToCoordonate(Point? map, PointF point, float zoom, bool multiplicate, bool divide)
		{
			var position = GetPointRotate180FlipY(new PointF((point.X / zoom), (point.Y / zoom)));
			position.X *= 7.875f;
			position.Y *= 7.875f;

			if (multiplicate)
			{
				position.X *= 8;
				position.Y *= 8;
			}
			if (divide)
			{
				position.X /= tileLenght;
				position.Y /= tileLenght;
			}
			if (map.HasValue)
			{
				position.X += map.Value.X * 16128;
				position.Y += map.Value.Y * 16128;
			}

			return position;
		}

		/// <summary>
		/// Convertion segment point to the game point
		/// </summary>
		/// <param name="map">Map coordonate</param>
		/// <param name="segmentNumber">Segment number</param>
		/// <param name="x">Segment coordonate x</param>
		/// <param name="y">Segment coordonate y</param>
		/// <returns>Map or game coordonate point</returns>
		public static PointF ConvertSegmentPointToCoordonate(Point? map, int segmentNumber, float x, float y)
		{
			var segmentX = segmentNumber % 64 * tileLenght * 6;
			var segmentY = segmentNumber / 64 * tileLenght * 6;
			var point = new PointF((x + segmentX), (y + segmentY));

			if (map.HasValue)
			{
				point.X += map.Value.X * 16128;
				point.Y += map.Value.Y * 16128;
			}

			return point;
		}

		/// <summary>
		/// Convertion points to the coordonnates
		/// </summary>
		/// <param name="points">List of polygon need to be convert</param>
		/// <param name="zoom">Actuel zoom</param>
		/// <param name="multiplicate">Need multiplication quotient</param>
		/// <param name="divide">Need to divide tile size</param>
		/// <returns>Map or game coordonate points</returns>
		public static PointF[] ConvertPointsToCoordonate(Point? map, PointF[] points, float zoom, bool multiplicate, bool divide)
		{
			var pts = (PointF[])points.Clone();
			for (var i = 0; i < pts.Length; i++)
				pts[i] = ConvertPointToCoordonate(map, pts[i], zoom, multiplicate, divide);

			return pts;
		}

		/// <summary>
		/// Get the center point between two point
		/// </summary>
		/// <param name="pt1">First point</param>
		/// <param name="pt2">Second point</param>
		/// <returns>Center point</returns>
		public static PointF GetCenterPoint(PointF pt1, PointF pt2)
		{
			var centerX = (Math.Max(pt1.X, pt2.X) - Math.Min(pt1.X, pt2.X)) / 2 + Math.Min(pt1.X, pt2.X);
			var centerY = (Math.Max(pt1.Y, pt2.Y) - Math.Min(pt1.Y, pt2.Y)) / 2 + Math.Min(pt1.Y, pt2.Y);
			return new PointF(centerX, centerY);
		}

		/// <summary>
		/// Get the center point list of the polygon
		/// </summary>
		/// <returns>Center point of the point list</returns>
		/// <returns></returns>
		public static PointF GetCenterPolygon(List<PointF> points) { return GetCenterPolygon(points.ToArray()); }

		/// <summary>
		/// Get the center point of the polygon
		/// </summary>
		/// <param name="polygon"></param>
		/// <returns>Center point of the polygon</returns>
		public static PointF GetCenterPolygon(Polygon2 polygon) { return GetCenterPolygon(polygon.ToPointArray()); }

		/// <summary>
		/// Get the center point of the point array
		/// </summary>
		/// <param name="points">Coordonate point</param>
		/// <returns>Center point of the polygon</returns>
		public static PointF GetCenterPolygon(PointF[] points)
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
			return new PointF(centerX, centerY);
		}

		/// <summary>
		/// Get area of the polygon
		/// </summary>
		/// <param name="points">Coordonate point</param>
		/// <returns>Total surface</returns>
		public static float GetPolygonArea(PointF[] points)
		{
			int len = points.Length;

			float area = 0;
			for (int i = 0; i < len; i++)
			{
				area +=
					(points[(i + 1) % len].X - points[i].X) *
					(points[(i + 1) % len].Y + points[i].Y) / 2;
			}
			return area;
		}

		/// <summary>
		/// Get point with rotate 180 and flip Y
		/// </summary>
		/// <param name="pointF">Coordonate point</param>
		/// <returns>Coordonate point</returns>
		public static PointF GetPointRotate180FlipY(PointF pointF)
		{
			var newPoint = new PointF(pointF.X, pointF.Y);
			newPoint.X = ImageManager.Width - newPoint.X;
			newPoint.Y = ImageManager.Height - newPoint.Y;
			newPoint.X = ImageManager.Width - newPoint.X;

			return newPoint;
		}
	}
}
