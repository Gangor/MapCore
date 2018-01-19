using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MapCore
{
	public class Nfl
	{
		#region Structure

		public K3DVector Direction { get; set; } = new K3DVector();
		public KColor Specular { get; set; } = new KColor();
		public KColor Diffuse { get; set; } = new KColor();
		public KColor Ambient { get; set; } = new KColor();
		public List<StructLights> Lights { get; set; } = new List<StructLights>();

		#endregion

		public MapManager Parent { get; }

		public event EventHandler<StructLights> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfl(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF point)
		{
			var light = new StructLights();
			light.Position = new K3DPosition(point.X, point.Y, 0f);
			Lights.Add(light);

			Added?.Invoke(this, light);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Specular = new KColor();
			Diffuse = new KColor();
			Ambient = new KColor(0, 255, 0, 255);
			Lights = new List<StructLights>();
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
					mem.Write(Direction.X);
					mem.Write(Direction.Y);
					mem.Write(Direction.Z);
					mem.Write(Specular.Color.B);
					mem.Write(Specular.Color.G);
					mem.Write(Specular.Color.R);
					mem.Write(Specular.Color.A);
					mem.Write(Diffuse.Color.B);
					mem.Write(Diffuse.Color.G);
					mem.Write(Diffuse.Color.R);
					mem.Write(Diffuse.Color.A);
					mem.Write(Ambient.Color.B);
					mem.Write(Ambient.Color.G);
					mem.Write(Ambient.Color.R);
					mem.Write(Ambient.Color.A);
					mem.Write(Lights.Count);

					for (int i = 0; i < Lights.Count; i++)
					{
						mem.Write(Lights[i].Position.X);
						mem.Write(Lights[i].Position.Y);
						mem.Write(Lights[i].Position.Z);
						mem.Write(Lights[i].Height);
						mem.Write(Lights[i].Direction.X);
						mem.Write(Lights[i].Direction.Y);
						mem.Write(Lights[i].Direction.Z);
						mem.Write(Lights[i].Specular.Color.B);
						mem.Write(Lights[i].Specular.Color.G);
						mem.Write(Lights[i].Specular.Color.R);
						mem.Write(Lights[i].Specular.Color.A);
						mem.Write(Lights[i].Diffuse.Color.B);
						mem.Write(Lights[i].Diffuse.Color.G);
						mem.Write(Lights[i].Diffuse.Color.R);
						mem.Write(Lights[i].Diffuse.Color.A);
						mem.Write(Lights[i].Ambient.Color.B);
						mem.Write(Lights[i].Ambient.Color.G);
						mem.Write(Lights[i].Ambient.Color.R);
						mem.Write(Lights[i].Ambient.Color.A);
						mem.Write((int)Lights[i].LightType);
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, string.Format("Nfl::GetBuffer<Exception> -> {0}\n", exception));
			}

			return null;
		}

		/// <summary>
		/// Load existing light
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					Direction.X = mem.ReadSingle();
					Direction.Y = mem.ReadSingle();
					Direction.Z = mem.ReadSingle();
					Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
					Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
					Ambient = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());

					var lightCount = mem.ReadInt32();

					for (int i = 0; i < lightCount; i++)
					{
						var light = new StructLights();
						light.Position.X = mem.ReadSingle();
						light.Position.Y = mem.ReadSingle();
						light.Position.Z = mem.ReadSingle();
						light.Height = mem.ReadSingle();
						light.Direction.X = mem.ReadSingle();
						light.Direction.Y = mem.ReadSingle();
						light.Direction.Z = mem.ReadSingle();
						light.Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
						light.Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
						light.Ambient = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
						light.LightType = (LightsType)mem.ReadInt32();
						Lights.Add(light);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, string.Format("Nfl::Load<Exception> -> {0}\n", exception));
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
		/// Remove light
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Lights.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
