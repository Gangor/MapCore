using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MapCore
{
	public class Nfc
	{
		public List<LocationInfo> Region { get; set; } = new List<LocationInfo>();

		public MapManager Parent { get; }

		public event EventHandler<LocationInfo> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfc(MapManager module)
		{
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points, PointF center)
		{
			var region = new LocationInfo();
			var polygon = Polygon2.FromPoints(points);

			region.X = center.X;
			region.Y = center.Y;
			region.Polygons.Add(polygon);
			Region.Add(region);

			Added?.Invoke(this, region);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Region = new List<LocationInfo>();
		}

		/// <summary>
		/// get buffer final file
		/// </summary>
		/// <returns></returns>
		public byte[] GetBuffer()
		{
			try
			{
				using (MemoryWriter mem = new MemoryWriter())
				{
					mem.Write(Region.Count);

					for (int i = 0; i < Region.Count; i++)
					{
						mem.Write(Region[i].Priority);
						mem.Write(Region[i].X);
						mem.Write(Region[i].Y);
						mem.Write(Region[i].Z);
						mem.Write(Region[i].Radius);
						mem.Write(Region[i].Description.Length);
						mem.Write(Encoding.Default.GetBytes(Region[i].Description));

						var script = Region[i].Scripts.Length == 0 ?
							Region[i].Scripts :
							Region[i].Scripts.Replace("\0", "") + '\0';

						mem.Write(script.Length);
						mem.Write(Encoding.Default.GetBytes(script));
						mem.Write(Region[i].Polygons.Count);

						for (int p = 0; p < Region[i].Polygons.Count; p++)
						{
							mem.Write(Region[i].Polygons[p].Count);

							for (int n = 0; n < Region[i].Polygons[p].Count; n++)
							{
								mem.Write(Region[i].Polygons[p][n].X);
								mem.Write(Region[i].Polygons[p][n].Y);
							}

						}
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfc::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing region
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					var regionCount = mem.ReadInt32();

					for (int i = 0; i < regionCount; i++)
					{
						var region = new LocationInfo();
						region.Priority = mem.ReadInt32();
						region.X = mem.ReadSingle();
						region.Y = mem.ReadSingle();
						region.Z = mem.ReadSingle();
						region.Radius = mem.ReadSingle();
						var DescriptionSize = mem.ReadInt32();
						region.Description = Encoding.Default.GetString(mem.ReadBytes(DescriptionSize));
						var ScriptSize = mem.ReadInt32();
						region.Scripts = Encoding.Default.GetString(mem.ReadBytes(ScriptSize));

						var polygonCount = mem.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon2();
							var pointNum = mem.ReadInt32();

							for (int n = 0; n < pointNum; n++)
							{
								var point = new K2DPosition(mem.ReadInt32(), mem.ReadInt32());
								polygon.Add(point);
							}

							region.Polygons.Add(polygon);
						}

						Region.Add(region);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfc::Load<Exception> -> {exception}\n");
			}
		}

		/// <summary>
		/// Refresh the current info for painting
		/// </summary>
		public void Refresh()
		{
			Painting?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Remove a region
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Region.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
