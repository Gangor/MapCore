using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MapCore
{
	public class Nfw
	{
		public List<Water> Waters { get; set; } = new List<Water>();

		public MapManager Parent { get; }

		public event EventHandler<Water> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfw(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF first, PointF last)
		{
			var water = new Water();
			var center = Utils.GetCenterPoint(first, last);
			water.Points[0] = new K3DPosition(first.X, first.Y, 0f);
			water.Points[1] = new K3DPosition(last.X, last.Y, 0f);
			water.Points[2] = new K3DPosition(center.X, center.Y, 0f);
			Waters.Add(water);

			Added?.Invoke(this, water);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Waters.Clear();
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
					mem.Write(Waters.Count);

					for (int i = 0; i < Waters.Count; i++)
					{
						mem.Write(Waters[i].Points[0].X);
						mem.Write(Waters[i].Points[0].Y);
						mem.Write(Waters[i].Points[0].Z);
						mem.Write(Waters[i].Points[1].X);
						mem.Write(Waters[i].Points[1].Y);
						mem.Write(Waters[i].Points[1].Z);
						mem.Write(Waters[i].Points[2].X);
						mem.Write(Waters[i].Points[2].Y);
						mem.Write(Waters[i].Points[2].Z);
						mem.Write(Waters[i].UseReflect);
						mem.Write(Waters[i].WaterId);
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfw::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing nflavor water
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					var WaterCount = mem.ReadInt32();

					for (int i = 0; i < WaterCount; i++)
					{
						var water = new Water();
						water.Points[0].X = mem.ReadSingle();
						water.Points[0].Y = mem.ReadSingle();
						water.Points[0].Z = mem.ReadSingle();
						water.Points[1].X = mem.ReadSingle();
						water.Points[1].Y = mem.ReadSingle();
						water.Points[1].Z = mem.ReadSingle();
						water.Points[2].X = mem.ReadSingle();
						water.Points[2].Y = mem.ReadSingle();
						water.Points[2].Z = mem.ReadSingle();
						water.UseReflect = mem.ReadInt32();
						water.WaterId = mem.ReadInt32();
						Waters.Add(water);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"NfwManager::Load<Exception> -> {exception}\n");
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
		/// Remove entry by index
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Waters.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
