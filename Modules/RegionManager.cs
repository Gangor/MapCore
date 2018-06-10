using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Region = MapCore.Models.Region;

namespace MapCore
{
	/// <summary>
	/// Management for collision (Nfa)
	/// </summary>
	public class RegionManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set the list of all region
		/// </summary>
		public List<Region> Regions { get; set; } = new List<Region>();


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
		public RegionManager(MapCore module)
		{
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="points"></param>
		public void Add(PointF[] points)
		{
			var center = Utils.GetCenterPolygon(points);
			var region = new Region
			{
				X = center.X,
				Y = center.Y,
				Z = center.Z
			};

			region.Polygons.Add(points);
			Regions.Add(region);

			Added?.Invoke(this, new AddedArgs(region, typeof(Region)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Regions = new List<Region>();
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
					mem.Write(Regions.Count);

					for (int i = 0; i < Regions.Count; i++)
					{
						mem.Write(Regions[i].Priority);
						mem.Write(Regions[i].X);
						mem.Write(Regions[i].Y);
						mem.Write(Regions[i].Z);
						mem.Write(Regions[i].Radius);
						mem.Write(Regions[i].Description.Length);
						mem.Write(Encoding.Default.GetBytes(Regions[i].Description));

						var script = Regions[i].Scripts.Length == 0 ?
							Regions[i].Scripts :
							Regions[i].Scripts.Replace("\0", "") + '\0';

						mem.Write(script.Length);
						mem.Write(Encoding.Default.GetBytes(script));
						mem.Write(Regions[i].Polygons.Count);

						for (int p = 0; p < Regions[i].Polygons.Count; p++)
						{
							mem.Write(Regions[i].Polygons[p].Count);

							for (int n = 0; n < Regions[i].Polygons[p].Count; n++)
							{
								mem.Write((int)Regions[i].Polygons[p][n].X);
								mem.Write((int)Regions[i].Polygons[p][n].Y);
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
						var region = new Region();
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
							var polygon = new Polygon();
							var pointNum = mem.ReadInt32();

							for (int n = 0; n < pointNum; n++)
							{
								var point = new Vector(mem.ReadInt32(), mem.ReadInt32());
								polygon.Add(point);
							}

							region.Polygons.Add(polygon);
						}

						Regions.Add(region);
					}
				}

				Render();
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
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Regions, typeof(Region)));
		}

		/// <summary>
		/// Remove a region
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Regions.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Region)));
		}

		/// <summary>
		/// Update a region
		/// </summary>
		/// <param name="index"></param>
		/// <param name="region"></param>
		public void Update(int index, Region region)
		{
			Regions[index] = region;

			Updated?.Invoke(this, new UpdatedArgs(index, region, typeof(Region)));
		}
	}
}
