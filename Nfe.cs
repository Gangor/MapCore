using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapCore
{
	public class Nfe
	{
		public List<EventAreaScript> Events { get; set; } = new List<EventAreaScript>();

		public MapManager Parent { get; }

		public event EventHandler<EventAreaScript> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfe(MapManager module)
		{
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points)
		{
			var eventArea = new EventAreaScript();
			var polygon = Polygon2.FromPoints(points);

			eventArea.Polygons.Add(polygon);
			Events.Add(eventArea);

			Added?.Invoke(this, eventArea);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Events.Clear();
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
					mem.Write(Events.Count);

					for (int i = 0; i < Events.Count; i++)
					{
						mem.Write(Events[i].AreaId);
						mem.Write(Events[i].Polygons.Count);

						for (int p = 0; p < Events[i].Polygons.Count; p++)
						{
							mem.Write(Events[i].Polygons[p].Count);

							for (int n = 0; n < Events[i].Polygons[p].Count; n++)
							{
								mem.Write(Events[i].Polygons[p][n].X);
								mem.Write(Events[i].Polygons[p][n].Y);
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
				Parent.Log(Levels.Fatal, $"Nfe::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing event area
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					var areaCount = mem.ReadInt32();

					for (int i = 0; i < areaCount; i++)
					{
						var area = new EventAreaScript();
						area.AreaId = mem.ReadInt32();
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

							area.Polygons.Add(polygon);
						}

						Events.Add(area);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfe::Load<Exception> -> {exception}\n");
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
			Events.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
