using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MapCore
{
	public class Nfs
	{
		#region Structure

		public string Sign { get; set; } = "nFlavor Script\0\0";
		public int Version { get; set; } = 2;
		public List<Location> Respawns { get; set; } = new List<Location>();
		public List<PropScriptInfo> Props { get; set; } = new List<PropScriptInfo>();
		
		#endregion

		public MapManager Parent { get; }
		public int[] SupportedVersion = { 2 };

		public event EventHandler<Location> AddedLocation;
		public event EventHandler<PropScriptInfo> AddedPropScript;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfs(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void Add(PointF first, PointF last)
		{
			var location = new Location();
			location.Left = (int)first.X;
			location.Top = (int)first.Y;
			location.Right = (int)last.X;
			location.Bottom = (int)last.Y;
			Respawns.Add(location);

			AddedLocation?.Invoke(this, location);
		}

		/// <summary>
		/// Add prop script
		/// </summary>
		/// <param name="propScript"></param>
		public void Add(PointF point)
		{
			var propScript = new PropScriptInfo();
			propScript.X = point.X;
			propScript.Y = point.Y;
			Props.Add(propScript);

			AddedPropScript?.Invoke(this, propScript);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Respawns.Clear();
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

					mem.Write(0); /* dwEventLocationOffset = */
					mem.Write(0); /* dwEventScriptOffset = */
					mem.Write(0); /* dwPropScriptOffset = */

					int dwEventLocationOffset = (int)mem.Position;
					mem.Write(Respawns.Count);

					for (int i = 0; i < Respawns.Count; i++)
					{
						mem.Write(Respawns[i].Left);
						mem.Write(Respawns[i].Top);
						mem.Write(Respawns[i].Right);
						mem.Write(Respawns[i].Bottom);
						mem.Write(Respawns[i].Description.Length);
						mem.Write(Encoding.Default.GetBytes(Respawns[i].Description));
					}

					int dwEventScriptOffset = (int)mem.Position;
					mem.Write(GetScriptCount());

					for (int i = 0; i < Respawns.Count; i++)
					{
						if (Respawns[i].Scripts.Count > 0)
						{
							mem.Write(i);
							mem.Write(Respawns[i].Scripts.Count);

							for (int f = 0; f < Respawns[i].Scripts.Count; f++)
							{
								mem.Write(f);
								mem.Write(Respawns[i].Scripts[f].Length);
								mem.Write(Encoding.Default.GetBytes(Respawns[i].Scripts[f]));
							}
						}
					}

					int dwPropScriptOffset = (int)mem.Position;
					mem.Write(Props.Count);

					for (int i = 0; i < Props.Count; i++)
					{
						mem.Write(Props[i].PropId);
						mem.Write(Props[i].X);
						mem.Write(Props[i].Y);
						mem.Write(Props[i].ModelId);
						mem.Write(Props[i].Scripts.Count);

						for (int f = 0; f < Props[i].Scripts.Count; f++)
						{
							mem.Write(f);
							mem.Write(Props[i].Scripts[f].Length);
							mem.Write(Encoding.Default.GetBytes(Props[i].Scripts[f]));
						}
					}

					mem.Seek(20, SeekOrigin.Begin);
					mem.Write(dwEventLocationOffset);
					mem.Write(dwEventScriptOffset);
					mem.Write(dwPropScriptOffset);

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfs::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Get total respawn script count
		/// </summary>
		/// <returns></returns>
		public int GetScriptCount()
		{
			int i = 0;
			foreach (var location in Respawns)
			{
				if (location.Scripts.Count != 0) i++;
			}
			return i;
		}

		/// <summary>
		/// Load existing nflavor script
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					Sign = Encoding.Default.GetString(mem.ReadBytes(16));
					Version = mem.ReadInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						Parent.Log(Levels.Error, $"Failed\n");
						Parent.Log(Levels.Fatal, $"Nfs::Load<Version> -> Incompatible version {Version} is not supported\n");
						return;
					}
#endif

					/* nfs.dwEventLocationOffset = */
					mem.ReadInt32();
					/* nfs.dwEventScriptOffset = */ mem.ReadInt32();
					/* nfs.dwPropScriptOffset = */ mem.ReadInt32();

					var nLocationCount = mem.ReadInt32();
					Respawns = new List<Location>();

					for (int i = 0; i < nLocationCount; i++)
					{
						var location = new Location();
						location.Left = mem.ReadInt32();
						location.Top = mem.ReadInt32();
						location.Right = mem.ReadInt32();
						location.Bottom = mem.ReadInt32();
						var stringSize = mem.ReadInt32();
						location.Description = Encoding.Default.GetString(mem.ReadBytes(stringSize));
						Respawns.Add(location);
					}

					var nScriptCount = mem.ReadInt32();

					for (int i = 0; i < nScriptCount; i++)
					{
						var index = mem.ReadInt32();
						var nFunctionCount = mem.ReadInt32();

						for (int f = 0; f < nFunctionCount; f++)
						{
							/* function.nTrigger = */ mem.ReadInt32();
							var nStringSize = mem.ReadInt32();
							var FunctionString = Encoding.Default.GetString(mem.ReadBytes(nStringSize));
							Respawns[index].Scripts.Add(FunctionString);
						}
					}

					var nPropCount = mem.ReadInt32();
					Props = new List<PropScriptInfo>();

					for (int i = 0; i < nPropCount; i++)
					{
						var propScript = new PropScriptInfo();
						propScript.PropId = mem.ReadInt32();
						propScript.X = mem.ReadSingle();
						propScript.Y = mem.ReadSingle();
						propScript.ModelId = mem.ReadInt16();
						var nFunctionCount = mem.ReadInt32();

						for (int f = 0; f < nFunctionCount; f++)
						{
							/* function.nTrigger = */ mem.ReadInt32();
							var nStringSize = mem.ReadInt32();
							var FunctionString = Encoding.Default.GetString(mem.ReadBytes(nStringSize));
							propScript.Scripts.Add(FunctionString);
						}

						Props.Add(propScript);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfs::Load<Exception> -> {exception}\n");
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
		/// Remove respawn nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveR(int index)
		{
			Respawns.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Remove prop nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveP(int index)
		{
			Props.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
