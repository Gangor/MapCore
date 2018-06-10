using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;

namespace MapCore
{
	/// <summary>
	/// Management for collision (Nfa)
	/// </summary>
	public class CollisionManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get the ratio of the point
		/// </summary>
		public int PointRatio { get; } = 8;

		/// <summary>
		/// Get or set the list of all collision
		/// </summary>
		public List<Polygon> Polygons { get; set; } = new List<Polygon>();


		#region Events

		/// <summary>
		/// Event when add objet
		/// </summary>
		public event EventHandler<AddedArgs> Added;

		/// <summary>
		/// Event when remove objet
		/// </summary>
		public event EventHandler<RemovedArgs> Removed;

		/// <summary>
		/// Event when render
		/// </summary>
		public event EventHandler<RenderArgs> Rendering;

		/// <summary>
		/// Event when update objet
		/// </summary>
		public event EventHandler<UpdatedArgs> Updated;

		#endregion

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public CollisionManager(MapCore parent)
		{
			Parent = parent;
		}

		/// <summary>
		/// Add new collision
		/// </summary>
		/// <param name="polygon"></param>
		public void Add(Polygon polygon)
		{
			Polygons.Add(polygon);

			Added?.Invoke(this, new AddedArgs(polygon, typeof(Light)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Polygons.Clear();
			Render();
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
						var vector = Polygons[i][p].Clone();

						vector.X = vector.X * Global.Scale * PointRatio / Global.TileLenght;
						vector.Y = vector.Y * Global.Scale * PointRatio / Global.TileLenght;
						vector = vector.Rotate180FlipY();

						mem.Write((int)vector.X);
						mem.Write((int)vector.Y);
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
						var polygon = new Polygon();
						var pointNum = mem.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var vector = new Vector
							{
								X = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale,
								Y = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale
							};
							polygon.Add(vector.Rotate180FlipY());
						}

						Polygons.Add(polygon);
					}
				}

				Render();
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
			Rendering?.Invoke(this, new RenderArgs(Polygons, typeof(Light)));
		}

		/// <summary>
		/// Remove a collision
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Polygons.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Light)));
		}

		/// <summary>
		/// Update a collision
		/// </summary>
		/// <param name="index"></param>
		/// <param name="polygon"></param>
		public void Update(int index, Polygon polygon)
		{
			Polygons[index] = polygon;

			Updated?.Invoke(this, new UpdatedArgs(index, polygon, typeof(Light)));
		}
	}
}
