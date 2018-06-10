using MapCore.Models;
using System;
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
		/// Get the map lenght
		/// </summary>
		public const int mapLenght = 16128;

		/// <summary>
		/// Get the tile lenght
		/// </summary>
		public const int tileLenght = 42;

		/// <summary>
		/// Convertion coordonate to the point
		/// </summary>
		/// <param name="point">Coordonate point</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="multiplicate">Need multiplication quotient</param>
		/// <param name="divide">Need to divide tile size</param>
		/// <returns>Game or Map point</returns>
		public static Vector ConvertCoordonateToPoint(Vector point, float zoom, bool divide, bool multiplicate)
		{
			var position = new Vector(point.X, point.Y, point.Z);

			position.X %= mapLenght;
			position.Y %= mapLenght;

			if (zoom != 1)
			{
				position.X /= zoom;
				position.Y /= zoom;
			}

			position = GetPointRotate180FlipY(position);

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
		public static (int, Vector) ConvertCoordonateToSegmentPoint(Point? map, Vector point)
		{
			var pt = point.Clone();

			pt.X %= mapLenght;
			pt.Y %= mapLenght;

			int segmentX = ((int)pt.X / (tileLenght * 6));
			segmentX = (segmentX == 64) ? 63 : segmentX;

			int segmentY = ((int)pt.Y / (tileLenght * 6));
			segmentY = (segmentY == 64) ? 63 : segmentY;

			int segmentNumber = segmentX + segmentY * 64;

			pt.X = point.X % (tileLenght * 6);
			pt.Y = point.Y % (tileLenght * 6);

			return (segmentNumber, pt);
		}

		/// <summary>
		/// Convertion coordonates to the points
		/// </summary>
		/// <param name="points">Coordonates points</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="hasFragmentation">Need to divide tile size</param>
		/// <param name="hasTile">Need multiplication quotient</param>
		/// <returns></returns>
		public static Vector[] ConvertCoordonatesToPoints(Vector[] points, float zoom, bool hasFragmentation, bool hasTile)
		{
			var data = new Vector[points.Length];
			for (int i = 0; i < points.Length; i++)
				data[i] = ConvertCoordonateToPoint(points[i], zoom, hasFragmentation, hasTile);

			return data;
		}

		/// <summary>
		/// Convertion point to the coordonate
		/// </summary>
		/// <param name="map">Map location</param>
		/// <param name="point">Map coordonate</param>
		/// <param name="zoom">Current zoom</param>
		/// <param name="hasFragmentation">Need multiplication quotient</param>
		/// <param name="hasTile">Need to divide tile size</param>
		/// <returns>Map or game coordonate point</returns>
		public static Vector ConvertPointToCoordonate(Point? map, Vector point, float zoom, bool hasFragmentation, bool hasTile)
		{
			var position = GetPointRotate180FlipY(new Vector((point.X / zoom), (point.Y / zoom)));

			position.X *= 7.875f;
			position.Y *= 7.875f;

			if (hasFragmentation)
			{
				position.X *= 8;
				position.Y *= 8;
			}
			if (hasTile)
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
		public static Vector ConvertSegmentPointToCoordonate(Point? map, int segmentNumber, float x, float y)
		{
			var segmentX = segmentNumber % 64 * tileLenght * 6;
			var segmentY = segmentNumber / 64 * tileLenght * 6;
			var point = new Vector((x + segmentX), (y + segmentY));

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
		/// <param name="map">Map location</param>
		/// <param name="points">List of polygon need to be convert</param>
		/// <param name="zoom">Actuel zoom</param>
		/// <param name="multiplicate">Need multiplication quotient</param>
		/// <param name="divide">Need to divide tile size</param>
		/// <returns>Map or game coordonate points</returns>
		public static Vector[] ConvertPointsToCoordonate(Point? map, Vector[] points, float zoom, bool multiplicate, bool divide)
		{
			var pts = (Vector[])points.Clone();
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
		public static Vector GetCenterPoint(Vector pt1, Vector pt2)
		{
			var centerX = (Math.Max(pt1.X, pt2.X) - Math.Min(pt1.X, pt2.X)) / 2 + Math.Min(pt1.X, pt2.X);
			var centerY = (Math.Max(pt1.Y, pt2.Y) - Math.Min(pt1.Y, pt2.Y)) / 2 + Math.Min(pt1.Y, pt2.Y);
			return new Vector(centerX, centerY);
		}

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

		/// <summary>
		/// Get point with rotate 180 and flip Y
		/// </summary>
		/// <param name="pointF">Coordonate point</param>
		/// <returns>Coordonate point</returns>
		public static Vector GetPointRotate180FlipY(Vector pointF)
		{
			var newPoint = new Vector(pointF.X, pointF.Y);

			newPoint.X = mapLenght - newPoint.X;
			newPoint.Y = mapLenght - newPoint.Y;
			newPoint.X = mapLenght - newPoint.X;

			return newPoint;
		}
	}
}
