using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;

namespace MapCore
{
	/// <summary>
	/// Management for event area (Nfe)
	/// </summary>
	public class EventAreaManager
	{
		/// <summary>
		/// Get or set the list of all event area
		/// </summary>
		public List<EventArea> Areas { get; set; } = new List<EventArea>();

		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get the ratio of the point
		/// </summary>
		public int PointRatio { get; } = 8;


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
		public EventAreaManager(MapCore module)
		{
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="polygon"></param>
		public void Add(Polygon polygon)
		{
			var eventArea = new EventArea();
			eventArea.Polygons.Add(polygon);
			Areas.Add(eventArea);

			Added?.Invoke(this, new AddedArgs(eventArea, typeof(Light)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Areas.Clear();
			Render();
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
					mem.Write(Areas.Count);

					for (int i = 0; i < Areas.Count; i++)
					{
						mem.Write(Areas[i].AreaId);
						mem.Write(Areas[i].Polygons.Count);

						for (int p = 0; p < Areas[i].Polygons.Count; p++)
						{
							mem.Write(Areas[i].Polygons[p].Count);

							for (int n = 0; n < Areas[i].Polygons[p].Count; n++)
							{
								var vector = Areas[i].Polygons[p][n].Clone();

								vector.X = vector.X * Global.Scale * PointRatio / Global.TileLenght;
								vector.Y = vector.Y * Global.Scale * PointRatio / Global.TileLenght;
								vector = vector.Rotate180FlipY();

								mem.Write((int)vector.X);
								mem.Write((int)vector.Y);
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
						var area = new EventArea();
						area.AreaId = mem.ReadInt32();
						var polygonCount = mem.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon();
							var pointNum = mem.ReadInt32();

							for (int n = 0; n < pointNum; n++)
							{
								var vector = new Vector
								{
									X = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale,
									Y = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale
								};
								polygon.Add(vector.Rotate180FlipY());
							}

							area.Polygons.Add(polygon);
						}

						Areas.Add(area);
					}
				}

				Render();
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
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Areas, typeof(Light)));
		}

		/// <summary>
		/// Remove a event area
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Areas.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Light)));
		}

		/// <summary>
		/// Update a event area
		/// </summary>
		/// <param name="index"></param>
		/// <param name="area"></param>
		public void Update(int index, EventArea area)
		{
			Areas[index] = area;

			Updated?.Invoke(this, new UpdatedArgs(index, area, typeof(Light)));
		}
	}
}
