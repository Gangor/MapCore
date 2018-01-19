using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapCore
{
	public class Nfp
	{
		public List<RecordNfp> Records { get; set; } = new List<RecordNfp>();

		public MapManager Parent { get; }

		public event EventHandler<RecordNfp> Added;
		public event EventHandler<EventArgs> Painting;
		public event EventHandler<EventArgs> Removed;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfp(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Add new regions
		/// </summary>
		/// <param name="location"></param>
		public void AddNfp(RecordNfp record)
		{
			Records.Add(record);

			Added?.Invoke(this, record);
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Records = new List<RecordNfp>();
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
								mem.Write(Records[i].Polygons[p][t].X);
								mem.Write(Records[i].Polygons[p][t].Y);
								mem.Write(Records[i].Polygons[p][t].Z);
							}
						}

						var description = Records[i].Description.Length == 0 ?
							Records[i].Description :
							Records[i].Description.Replace("\0", "") + '\0';

						mem.Write(description.Length);
						mem.Write(Encoding.Default.GetBytes(description));
					}

					Parent.Log(Levels.Good, "Ok\n");
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
						var nfp = new RecordNfp();
						nfp.Id = mem.ReadInt32();
						var polygonCount = mem.ReadInt32();

						for (int p = 0; p < polygonCount; p++)
						{
							var polygon = new Polygon3();
							var pointNum = mem.ReadInt32();

							for (int t = 0; t < pointNum; t++)
							{
								var point = new K3DPosition(mem.ReadSingle(), mem.ReadSingle(), mem.ReadSingle());
								polygon.Add(point);
							}

							nfp.Polygons.Add(polygon);
						}

						var DescriptionCount = mem.ReadInt32();
						nfp.Description = Encoding.Default.GetString(mem.ReadBytes(DescriptionCount));

						Records.Add(nfp);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfp::Load<Exception> -> {exception}\n");
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
		/// Remove record from list
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			Records.RemoveAt(index);

			Removed?.Invoke(this, new EventArgs());
		}
	}
}
