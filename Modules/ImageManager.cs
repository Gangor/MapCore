using DataCore;
using MapCore.Enum;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MapCore
{
	/// <summary>
	/// Manage the map picture
	/// </summary>
	public class ImageManager
	{
		/// <summary>
		/// Get the pictures width
		/// </summary>
		public const int Width = 2048;

		/// <summary>
		/// Get the pictures height
		/// </summary>
		public const int Height = 2048;

		/// <summary>
		/// Bitmap for cache blank map
		/// </summary>
		public Bitmap Cache = new Bitmap(Width, Height);

		/// <summary>
		/// Bitmap for draw all element
		/// </summary>
		public Bitmap Picture = new Bitmap(Width, Height);

		/// <summary>
		/// Manage bitmap for draw element
		/// </summary>
		private Graphics graphic;

		/// <summary>
		/// Module parent
		/// </summary>
		public MapCore Parent;

		#region Constructor

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public ImageManager(MapCore parent)
		{
			graphic = Graphics.FromImage(Picture);
			Parent = parent;
			New();
		}

		#endregion

		#region Draw Method

		/// <summary>
		/// Draw image on current pictures
		/// </summary>
		public void DrawCache()
		{
			graphic.DrawImage(Cache, 0, 0, Cache.Width, Cache.Height);
		}

		/// <summary>
		/// Make text center
		/// </summary>
		/// <param name="s"></param>
		/// <param name="font"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void DrawCenterString(string s, Font font, Brush brush, float x, float y)
		{
			var size = graphic.MeasureString(s, font);
			DrawString(s, font, brush, x - (size.Width / 2f), y + (size.Height / 2f));
		}

		/// <summary>
		/// Make rectangle with cross
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		public void DrawCross(Pen pen, PointF pt1, PointF pt2)
		{
			var pt3 = new PointF(pt1.X, pt2.Y);
			var pt4 = new PointF(pt2.X, pt1.Y);

			// Cossing
			graphic.DrawLine(pen, pt3, pt4);
			graphic.DrawLine(pen, pt2, pt1);
		}

		/// <summary>
		/// Make rectangle with cross
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		public void DrawCrossRectangle(Pen pen, PointF pt1, PointF pt2)
		{
			DrawRectangle(pen, pt1, pt2);
			DrawCross(pen, pt1, pt2);
		}

		/// <summary>
		/// Make point
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="point"></param>
		public void DrawPoint(Pen pen, PointF point)
		{
			graphic.DrawEllipse(pen, new RectangleF(point.X - 2, point.Y - 2, 4, 4));
			graphic.FillEllipse(pen.Brush, new RectangleF(point.X - 2, point.Y - 2, 4, 4));
		}

		/// <summary>
		/// Make polygon
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="points"></param>
		public void DrawPolygon(Pen pen, PointF[] points)
		{
			graphic.DrawPolygon(pen, points);
		}

		/// <summary>
		/// Make rectangle
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		public void DrawRectangle(Pen pen, PointF pt1, PointF pt2)
		{
			var pointA = new PointF(pt1.X, pt2.Y);
			var pointD = new PointF(pt2.X, pt1.Y);

			graphic.DrawLine(pen, pointA, pt2);
			graphic.DrawLine(pen, pt2, pointD);
			graphic.DrawLine(pen, pointD, pt1);
			graphic.DrawLine(pen, pt1, pointA);
		}

		/// <summary>
		/// Make text center
		/// </summary>
		/// <param name="s"></param>
		/// <param name="font"></param>
		/// <param name="brush"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void DrawString(string s, Font font, Brush brush, float x, float y)
		{
			graphic.DrawString(s, font, brush, x, y);
		}

		#endregion

		/// <summary>
		/// Create new picture
		/// </summary>
		public void New()
		{
			using (var g = Graphics.FromImage(Cache))
			{
				g.Clear(Color.FromArgb(255, 120, 146, 173));
				DrawCache();
			}
		}

		/// <summary>
		/// Load the full map with file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <param name="encoding"></param>
		public void Load(string path, string name, string encoding)
		{
			Load(core:null, path: path, name: name, encoding:encoding, useCore: false);
		}

		/// <summary>
		/// Load the full map with core
		/// </summary>
		/// <param name="core"></param>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <param name="encoding"></param>
		public void Load(Core core, string path, string name, string encoding)
		{
			Load(core: core, path: path, name: name, encoding: encoding, useCore: true);
		}

		/// <summary>
		/// Load the full map
		/// </summary>
		/// 
		/// <example>
		/// Version 1 : 2048 % 128 = 16
		/// Version 2 : 2048 % 256 = 8
		/// </example>
		/// 
		/// <param name="core"></param>
		/// <param name="path">The location to jpg dump</param>
		/// <param name="name">Associate name file</param>
		/// <param name="encoding"></param>
		/// <param name="useCore"></param>
		private void Load(Core core, string path, string name, string encoding, bool useCore = false)
		{
			New();

			if (!Directory.Exists(path))
			{
				Parent.Log(Levels.Warning, $"jpg::Load<folder>() => Missing folder {path}.\n");
				return;
			}

			Parent.Log(Levels.Info, $"Loading minimap...\t");

			try
			{
				var error = 0;
				var load = 0;

				using (var g = Graphics.FromImage(Cache))
				{
					int partX;
					int partY;
					var partHeight = 128;
					var partWidth = 128;
					var prefix = string.Empty;

					var occurence = useCore ? core.GetEntriesByPartialName($"v256_{name}*").Select(r => r.Name)
						: Directory.GetFiles(path, $"v256_{name}*");

					if (occurence.Count() > 0)
					{
						partHeight = 256;
						partWidth = 256;
						prefix = "v256_";
					}

					partX = Width / partWidth;
					partY = Height / partHeight;

					g.Clear(Color.FromArgb(255, 120, 146, 173));

					for (int y = 0; y < partY; y++)
					{
						for (int x = 0; x < partX; x++)
						{
							var filename = $"{prefix}{name}_{y}_{x}{encoding}.jpg";
							var exist = useCore ? occurence.Any(r => r == filename)
								: File.Exists(Path.Combine(path, filename));

							if (!exist)
							{
								filename = encoding != "" ? filename.Replace(encoding, string.Empty) : filename;
							}

							if (!useCore && !File.Exists(filename) || useCore && !occurence.Any(r => r == filename))
							{
								error++;
								continue;
							}
							
							var image = useCore ? Image.FromStream(new MemoryStream(core.GetFileBytes(filename)))
								: Image.FromFile(filename);

							g.DrawImage(image, x * partWidth, y * partHeight, partWidth, partHeight);

							load++;
						}
					}

					Cache.RotateFlip(RotateFlipType.Rotate180FlipX);
					DrawCache();

					if (error == 0)		Parent.Log(Levels.Good, "Ok\n");
					else if (load > 0)	Parent.Log(Levels.Warning, $"Ok (Partial count : {load}/{partX * partX})\n");
					else				Parent.Log(Levels.Error, "Failed\n");
				}

				Parent.Log(Levels.Info, $"Loading the minimap completed. (Error count : {error})\n");
			}
			catch (Exception exception)
			{
				New();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"jpg::Load<Exception> -> {exception}\n");
			}
		}
	}
}
