using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Management for script (Nfs)
	/// </summary>
	public class ScriptManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set the list of all npc prop
		/// </summary>
		public List<NpcProp> Props { get; set; } = new List<NpcProp>();

		/// <summary>
		/// Get or set the list of all respawn
		/// </summary>
		public List<Respawn> Respawns { get; set; } = new List<Respawn>();

		/// <summary>
		/// Get the signature
		/// </summary>
		public string Sign { get; } = "nFlavor Script\0\0";

		/// <summary>
		/// Get support version
		/// </summary>
		public int[] SupportedVersion { get; } = { 2 };

		/// <summary>
		/// Get or set the version
		/// </summary>
		public int Version { get; set; } = 2;


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
		public ScriptManager(MapCore parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add prop script
		/// </summary>
		/// <param name="point"></param>
		public void AddNpc(PointF point)
		{
			var propScript = new NpcProp
			{
				X = point.X,
				Y = point.Y
			};
			Props.Add(propScript);

			Added?.Invoke(this, new AddedArgs(propScript, typeof(NpcProp)));
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="first"></param>
		/// <param name="last"></param>
		public void AddRespawn(PointF first, PointF last)
		{
			var location = new Respawn
			{
				Left = (int)first.X,
				Top = (int)first.Y,
				Right = (int)last.X,
				Bottom = (int)last.Y
			};
			Respawns.Add(location);

			Added?.Invoke(this, new AddedArgs(location, typeof(Respawn)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Respawns.Clear();
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
					/*Sign = Encoding.Default.GetString()*/ mem.ReadBytes(16);
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
					Respawns = new List<Respawn>();

					for (int i = 0; i < nLocationCount; i++)
					{
						var location = new Respawn();
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
					Props = new List<NpcProp>();

					for (int i = 0; i < nPropCount; i++)
					{
						var propScript = new NpcProp();
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

				Render();
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
		public void Render()
		{
			RenderNpc();
			RenderRespawn();
		}

		/// <summary>
		/// Refresh the current info for painting
		/// </summary>
		public void RenderNpc()
		{
			Rendering?.Invoke(this, new RenderArgs(Props, typeof(NpcProp)));
		}

		/// <summary>
		/// Refresh the current info for painting
		/// </summary>
		public void RenderRespawn()
		{
			Rendering?.Invoke(this, new RenderArgs(Respawns, typeof(Respawn)));
		}

		/// <summary>
		/// Remove prop nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveNpc(int index)
		{
			Props.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(NpcProp)));
		}

		/// <summary>
		/// Remove respawn nflavor script
		/// </summary>
		/// <param name="index"></param>
		public void RemoveRespawn(int index)
		{
			Respawns.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Respawn)));
		}

		/// <summary>
		/// Update a prop nflavor script
		/// </summary>
		/// <param name="index"></param>
		/// <param name="prop"></param>
		public void UpdateNpc(int index, NpcProp prop)
		{
			Props[index] = prop;

			Updated?.Invoke(this, new UpdatedArgs(index, prop, typeof(NpcProp)));
		}

		/// <summary>
		/// Update a respawn nflavor script
		/// </summary>
		/// <param name="index"></param>
		/// <param name="respawn"></param>
		public void UpdateRespawn(int index, Respawn respawn)
		{
			Respawns[index] = respawn;

			Updated?.Invoke(this, new UpdatedArgs(index, respawn, typeof(Respawn)));
		}
	}
}
