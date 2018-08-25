using MapCore.Enum;
using MapCore.Events;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Management for unknow (Nfp)
	/// </summary>
	public class UnknowManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set the list of all unknow
		/// </summary>
		public List<Unknow> Records { get; set; } = new List<Unknow>();


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
		public UnknowManager(MapCore parent)
		{
			Dispose();
			Parent = parent;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="record"></param>
		public void Add(Unknow record)
		{
			Records.Add(record);

			Added?.Invoke(this, new AddedArgs(record, typeof(Unknow)));
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Dispose()
		{
			Records = new List<Unknow>();
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
					mem.Write(Records.Count);

					for (int i = 0; i < Records.Count; i++)
					{
						mem.Write(Records[i].Id);
						mem.Write(Records[i].Polygons.Count);

						for (int p = 0; p < Records[i].Polygons.Count; p++)
						{
							mem.Write(Records[i].Polygons[p].Count);

							for (int t = 0; t < Records[i].Polygons[p].Count; t++)
							{
								var vector = Records[i].Polygons[p][t].Clone();

								vector.X *= 7.875f;
								vector.Y *= 7.875f;
								vector = vector.Rotate180FlipY();

								mem.Write(vector.X);
								mem.Write(vector.Y);
								mem.Write(vector.Z);
							}
						}

						var description = Records[i].Description.Length == 0 ?
							Records[i].Description :
							Records[i].Description.Replace("\0", "") + '\0';

						mem.Write(description.Length);
						mem.Write(Encoding.Default.GetBytes(description));
					}

					Parent.Log(Levels.Success, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfp::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing unknown
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					var nfpCount = mem.ReadInt32();

					for (int i = 0; i < nfpCount; i++)
					{
						var nfp = new Unknow();
						nfp.Id = mem.ReadInt32();
						var polygonCount = mem.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon();
							var pointNum = mem.ReadInt32();

							for (int t = 0; t < pointNum; t++)
							{
								var vector = new Vector
								{
									X = mem.ReadSingle() / 7.875f,
									Y = mem.ReadSingle() / 7.875f,
									Z = mem.ReadSingle()
								};

								polygon.Add(vector.Rotate180FlipY());
							}

							nfp.Polygons.Add(polygon);
						}

						var DescriptionCount = mem.ReadInt32();
						nfp.Description = Encoding.Default.GetString(mem.ReadBytes(DescriptionCount));

						Records.Add(nfp);
					}
				}

				Render();
				Parent.Log(Levels.Success, "Ok\n");
			}
			catch (Exception exception)
			{
				Dispose();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfp::Load<Exception> -> {exception}\n");
			}
		}

		/// <summary>
		/// Refresh the current info for painting
		/// </summary>
		public void Render()
		{
			Rendering?.Invoke(this, new RenderArgs(Records, typeof(Unknow)));
		}

		/// <summary>
		/// Remove record from list
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Records.RemoveAt(index);

			Removed?.Invoke(this, new RemovedArgs(index, typeof(Unknow)));
		}

		/// <summary>
		/// Update a record
		/// </summary>
		/// <param name="index"></param>
		/// <param name="record"></param>
		public void UpdateRespawn(int index, Unknow record)
		{
			Records[index] = record;

			Updated?.Invoke(this, new UpdatedArgs(index, record, typeof(Unknow)));
		}
	}
}
