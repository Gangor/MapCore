using MapCore.Enum;
using MapCore.Events;
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

		public event EventHandler<AddedArgs> Added;
		public event EventHandler<RemovedArgs> Removed;
		public event EventHandler<RenderArgs> Rendering;
		public event EventHandler<UpdatedArgs> Updated;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfa(MapManager parent)
		{
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

			Added?.Invoke(this, new AddedArgs(Polygons, typeof(List<Polygon2>)));
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
			var mem = new MemoryWriter();
			try
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
			}
			catch (Exception exception)
			{
				mem.Clear();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfa::GetBuffer<Exception> -> {exception}\n");
			}

			return mem.GetBuffer();
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
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Polygons, typeof(List<Polygon2>)));
		}

		/// <summary>
		/// Remove a region
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Polygons.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Nfa)));
		}

		/// <summary>
		/// Update a region
		/// </summary>
		/// <param name="polygon"></param>
		public void Update(Polygon2 polygon)
		{
			var data = Polygons.Find(r => r == polygon);
			if (data != null)
			{
				data = polygon;
			}

			Updated?.Invoke(this, new UpdatedArgs(Polygons, typeof(List<Polygon2>)));
		}
	}
}
