using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;

namespace MapCore
{
	/// <summary>
	/// Management for collision (Nfl)
	/// </summary>
	public class LightManager
	{
		/// <summary>
		/// Get or set ambient color
		/// </summary>
		public KColor Ambient { get; set; } = new KColor();

		/// <summary>
		/// Get or set diffuse color
		/// </summary>
		public KColor Diffuse { get; set; } = new KColor();

		/// <summary>
		/// Get or set direction vector
		/// </summary>
		public Vector Direction { get; set; } = new Vector();

		/// <summary>
		/// Get or set the list of all light
		/// </summary>
		public List<Light> Lights { get; set; } = new List<Light>();

		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set specular color
		/// </summary>
		public KColor Specular { get; set; } = new KColor();


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
		public LightManager(MapCore parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new light
		/// </summary>
		/// <param name="vector"></param>
		public void Add(Vector vector)
		{
			var light = new Light
			{
				Position = vector
			};
			Lights.Add(light);

			Added?.Invoke(this, new AddedArgs(light, typeof(Light)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Specular = new KColor();
			Diffuse = new KColor();
			Ambient = new KColor(0, 255, 0, 255);
			Lights = new List<Light>();
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
						var vector = Lights[i].Position.Clone();

						vector.X *= 7.875f;
						vector.Y *= 7.875f;
						vector = vector.Rotate180FlipY();

						mem.Write(vector.X);
						mem.Write(vector.Y);
						mem.Write(vector.Z);
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
						var light = new Light();
						var vector = new Vector
						{
							X = mem.ReadSingle() / 7.875f,
							Y = mem.ReadSingle() / 7.875f,
							Z = mem.ReadSingle()
						};

						light.Position = vector.Rotate180FlipY();
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

				Render();
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
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Lights, typeof(Light)));
		}

		/// <summary>
		/// Remove light
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Lights.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Light)));
		}

		/// <summary>
		/// Update a light
		/// </summary>
		/// <param name="index"></param>
		/// <param name="light"></param>
		public void Update(int index, Light light)
		{
			Lights[index] = light;

			Updated?.Invoke(this, new UpdatedArgs(index, light, typeof(Light)));
		}
	}
}
