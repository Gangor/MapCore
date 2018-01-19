using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapCore
{
	public class Nfa
	{
		public List<Polygon2> Polygons { get; set; } = new List<Polygon2>();

		public MapManager Parent { get; }

		public event EventHandler<Polygon2> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfa(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new collision
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF[] points)
		{
			var polygon = Polygon2.FromPoints(points);
			Polygons.Add(polygon);

			Added?.Invoke(this, polygon);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Polygons.Clear();
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
					mem.Write(Polygons.Count);

					for (int i = 0; i < Polygons.Count; i++)
					{
						mem.Write(Polygons[i].Count);

						for (int p = 0; p < Polygons[i].Count; p++)
						{
							mem.Write(Polygons[i][p].X);
							mem.Write(Polygons[i][p].Y);
						}
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfa::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing collision
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					var polygonCount = mem.ReadInt32();

					for (int i = 0; i < polygonCount; i++)
					{
						var polygon = new Polygon2();
						var pointNum = mem.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var point = new K2DPosition(mem.ReadInt32(), mem.ReadInt32());
							polygon.Add(point);
						}

						Polygons.Add(polygon);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfa::Load<Exception> -> {exception}\n");
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
			Polygons.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
