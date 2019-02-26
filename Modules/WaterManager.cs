﻿using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapCore
{
	/// <summary>
	/// Management for water (Nfw)
	/// </summary>
	public class WaterManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }
		
		/// <summary>
		/// Get or set the list of all water
		/// </summary>
		public List<Water> Waters { get; set; } = new List<Water>();


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
		public WaterManager(MapCore parent)
		{
			Dispose();
			Parent = parent;
		}

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="first"></param>
		/// <param name="last"></param>
		public void Add(Vector first, Vector last)
		{
			var water = new Water
			{
				Rectangle = new RectangleVector
				{
					LeftTop = first,
					RightBottom = last
				}
			};

			water.Rectangle.Center = water.Rectangle.GetCenterPoint();
			Waters.Add(water);

			Added?.Invoke(this, new AddedArgs(water, typeof(Water)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Dispose()
		{
			Waters.Clear();
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
					mem.Write(Waters.Count);

					for (int i = 0; i < Waters.Count; i++)
					{
						var rectangle = Waters[i].Rectangle.Clone();

						rectangle.LeftTop.X *= Global.AttrLenght;
						rectangle.LeftTop.Y *= Global.AttrLenght;
						rectangle.LeftTop = rectangle.LeftTop.Rotate180FlipY();

						rectangle.RightBottom.X *= Global.AttrLenght;
						rectangle.RightBottom.Y *= Global.AttrLenght;
						rectangle.RightBottom = rectangle.RightBottom.Rotate180FlipY();

						rectangle.Center.X *= Global.AttrLenght;
						rectangle.Center.Y *= Global.AttrLenght;
						rectangle.Center = rectangle.RightBottom.Rotate180FlipY();

						mem.Write((int)rectangle.LeftTop.X);
						mem.Write((int)rectangle.LeftTop.Y);
						mem.Write((int)rectangle.LeftTop.Z);
						mem.Write((int)rectangle.RightBottom.X);
						mem.Write((int)rectangle.RightBottom.Y);
						mem.Write((int)rectangle.RightBottom.Z);
						mem.Write((int)rectangle.Center.X);
						mem.Write((int)rectangle.Center.Y);
						mem.Write((int)rectangle.Center.Z);
						mem.Write(Waters[i].UseReflect);
						mem.Write(Waters[i].WaterId);
					}

					Parent.Log(Levels.Success, "Ok\n");
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

						water.Rectangle.LeftTop = new Vector
						{
							X = mem.ReadSingle() / Global.AttrLenght,
							Y = mem.ReadSingle() / Global.AttrLenght,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

						water.Rectangle.RightBottom = new Vector
						{
							X = mem.ReadSingle() / Global.AttrLenght,
							Y = mem.ReadSingle() / Global.AttrLenght,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

						water.Rectangle.Center = new Vector
						{
							X = mem.ReadSingle() / Global.AttrLenght,
							Y = mem.ReadSingle() / Global.AttrLenght,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

						water.UseReflect = mem.ReadInt32();
						water.WaterId = mem.ReadInt32();
						Waters.Add(water);
					}
				}

				Render();
				Parent.Log(Levels.Success, "Ok\n");
			}
			catch (Exception exception)
			{
				Dispose();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"NfwManager::Load<Exception> -> {exception}\n");
			}
		}

		/// <summary>
		/// Refresh the current info for painting
		/// </summary>
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Waters, typeof(Water)));
		}

		/// <summary>
		/// Remove entry by index
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Waters.RemoveAt(index);
			
			Removed?.Invoke(this, new RemovedArgs(index, typeof(Water)));
		}

		/// <summary>
		/// Update a record
		/// </summary>
		/// <param name="index"></param>
		/// <param name="water"></param>
		public void Update(int index, Water water)
		{
			Waters[index] = water;

			Updated?.Invoke(this, new UpdatedArgs(index, water, typeof(Water)));
		}
	}
}
