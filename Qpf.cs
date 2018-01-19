using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MapCore
{
	public class Qpf
	{
		#region Structure

		public string Sign { get; set; } = "nFlavor QuestProp\0";
		public uint Version { get; set; } = 3;
		public List<Prop> Props { get; set; } = new List<Prop>();

		#endregion

		public MapManager Parent { get; }
		public uint[] SupportedVersion = { 1, 3 };

		public event EventHandler<Prop> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Qpf(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF point)
		{
			var prop = new Prop();
			prop.X = point.X;
			prop.Y = point.Y;
			Props.Add(prop);

			Added?.Invoke(this, prop);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Props.Clear();
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
					mem.Write(Encoding.Default.GetBytes(Sign));
					mem.Write(Version);
					mem.Write(Props.Count);

					for (int i = 0; i < Props.Count; i++)
					{
						mem.Write(Props[i].QuestPropID);
						mem.Write(Props[i].X);
						mem.Write(Props[i].Y);
						mem.Write(Props[i].OffSet);
						mem.Write(Props[i].RotateX);
						mem.Write(Props[i].RotateY);
						mem.Write(Props[i].RotateZ);
						mem.Write(Props[i].ScaleX);
						mem.Write(Props[i].ScaleY);
						mem.Write(Props[i].ScaleZ);
						mem.Write(Props[i].PropNum);
						mem.Write(Props[i].LockedHeight);
						mem.Write(Props[i].LockHeight);
						mem.Write(Props[i].TextureGroupIndex);
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Qpf::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing nflavor props
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					Sign = Encoding.Default.GetString(mem.ReadBytes(18));
					Version = mem.ReadUInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						Parent.Log(Levels.Error, "Failed\n");
						Parent.Log(Levels.Fatal, $"Incompatible version {Version} is not supported or not implemented.\n");
						return;
					}
#endif

					var PropCount = mem.ReadInt32();

					for (int i = 0; i < PropCount; i++)
					{
						var prop = new Prop();
						prop.QuestPropID = mem.ReadInt32();
						prop.X = mem.ReadSingle();
						prop.Y = mem.ReadSingle();
						prop.OffSet = mem.ReadSingle();
						prop.RotateX = mem.ReadSingle();
						prop.RotateY = mem.ReadSingle();
						prop.RotateZ = mem.ReadSingle();
						prop.ScaleX = mem.ReadSingle();
						prop.ScaleY = (Version >= 3) ? mem.ReadSingle() : prop.ScaleX;
						prop.ScaleZ = (Version >= 3) ? mem.ReadSingle() : prop.ScaleX;
						prop.PropNum = mem.ReadUInt16();
						prop.LockedHeight = (Version >= 3) ? mem.ReadBoolean() : false;
						prop.LockHeight = (Version >= 3) ? mem.ReadSingle() : 0f;
						prop.TextureGroupIndex = (Version >= 3) ? mem.ReadInt16() : (short)-1;
						Props.Add(prop);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Qpf::Load<Exception> -> {exception}\n");
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
			Props.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
