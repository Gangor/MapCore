using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Management for quest prop (Qpf)
	/// </summary>
	public class QuestPropManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set the list of all quest prop
		/// </summary>
		public List<QuestProp> Props { get; set; } = new List<QuestProp>();

		/// <summary>
		/// Get the signature
		/// </summary>
		public string Sign { get; } = "nFlavor QuestProp\0";

		/// <summary>
		/// Get support version
		/// </summary>
		public uint[] SupportedVersion { get; } = { 1, 3 };

		/// <summary>
		/// Get or set the version
		/// </summary>
		public uint Version { get; set; } = 3;
		

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
		public QuestPropManager(MapCore parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new prop
		/// </summary>
		/// <param name="vector"></param>
		public void Add(Vector vector)
		{
			var prop = new QuestProp
			{
				Position = vector
			};
			Props.Add(prop);

			Added?.Invoke(this, new AddedArgs(prop, typeof(QuestProp)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Props.Clear();
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
					mem.Write(Encoding.Default.GetBytes(Sign));
					mem.Write(Version);
					mem.Write(Props.Count);

					for (int i = 0; i < Props.Count; i++)
					{
						mem.Write(Props[i].QuestPropID);

						var vector = Props[i].Position.Clone();

						vector.X *= 7.875f;
						vector.Y *= 7.875f;
						vector = vector.Rotate180FlipY();

						mem.Write(vector.X);
						mem.Write(vector.Y);
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
					/* Sign = Encoding.Default.GetString()*/ mem.ReadBytes(18);
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
						var prop = new QuestProp();
						prop.QuestPropID = mem.ReadInt32();

						var vector = new Vector
						{
							X = mem.ReadSingle() / 7.875f,
							Y = mem.ReadSingle() / 7.875f
						};

						prop.Position = vector.Rotate180FlipY();
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

				Render();
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
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Props, typeof(QuestProp)));
		}

		/// <summary>
		/// Remove a quest prop
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Props.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(QuestProp)));
		}

		/// <summary>
		/// Update a quest prop
		/// </summary>
		/// <param name="index"></param>
		/// <param name="prop"></param>
		public void Update(int index, QuestProp prop)
		{
			Props[index] = prop;

			Updated?.Invoke(this, new UpdatedArgs(index, prop, typeof(QuestProp)));
		}
	}
}
