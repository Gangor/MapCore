using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapCore
{
	public class Pvs
	{
		#region Structure
		
		public string Sign { get; set; } = "RAPPELZ PVS\0\0\0\0\0";
		public ushort Version { get; set; } = 1;
		public byte SegmentCountPerMap { get; set; }
		public byte MapStartPosX { get; set; }
		public byte MapStartPosY { get; set; }
		public byte SegmentLeft { get; set; }
		public byte SegmentTop { get; set; }
		public byte SegmentRight { get; set; }
		public byte SegmentBottom { get; set; }
		public List<PVS_SEGMENT_V1> Segments { get; set; } = new List<PVS_SEGMENT_V1>();
		public List<PVS_PROP_V1> Props { get; set; } = new List<PVS_PROP_V1>();

		#endregion

		public MapManager Parent { get; }
		public int[] SupportedVersion = { 1 };

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Pvs(MapManager parent)
		{
			Blank();
			Parent = parent;
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			SegmentCountPerMap = 64;
			MapStartPosX = 0;
			MapStartPosY = 0;
			SegmentLeft = 0;
			SegmentTop = 0;
			SegmentRight = 0;
			SegmentBottom = 0;
			Segments = new List<PVS_SEGMENT_V1>();
			Props = new List<PVS_PROP_V1>();
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
					mem.Write(SegmentCountPerMap);
					mem.Write(MapStartPosX);
					mem.Write(MapStartPosY);
					mem.Write((ushort)Segments.Count);
					mem.Write(Props.Count);
					mem.Write(SegmentLeft);
					mem.Write(SegmentTop);
					mem.Write(SegmentRight);
					mem.Write(SegmentBottom);

					for (int i = 0; i < Segments.Count; i++)
					{
						mem.Write(Segments[i].SegmentX);
						mem.Write(Segments[i].SegmentY);
						mem.Write((ushort)Segments[i].IncludeSegments.Count);

						for (int f = 0; f < Segments[i].IncludeSegments.Count; f++)
						{
							mem.Write(Segments[i].IncludeSegments[f].SegmentX);
							mem.Write(Segments[i].IncludeSegments[f].SegmentY);
						}
					}

					for (int i = 0; i < Props.Count; i++)
					{
						mem.Write(Props[i].SegmentX);
						mem.Write(Props[i].SegmentY);
						mem.Write(Props[i].IncludeProps.Count);

						for (int f = 0; f < Props[i].IncludeProps.Count; f++)
						{
							mem.Write(Props[i].IncludeProps[f].PropIdx);
							mem.Write(Props[i].IncludeProps[f].SegmentIdx);
						}
					}

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}
			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Pvs::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing potencially visible set
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					Sign = Encoding.Default.GetString(mem.ReadBytes(16));
					Version = mem.ReadUInt16();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						Parent.Log(Levels.Error, $"Failed\n");
						Parent.Log(Levels.Fatal, $"Incompatible version {Version} is not supported or not implemented.\n");
						return;
					}
#endif

					SegmentCountPerMap = mem.ReadByte();
					MapStartPosX = mem.ReadByte();
					MapStartPosY = mem.ReadByte();

					var segmentCount = mem.ReadUInt16();
					var propCount = mem.ReadInt32();

					SegmentLeft = mem.ReadByte();
					SegmentTop = mem.ReadByte();
					SegmentRight = mem.ReadByte();
					SegmentBottom = mem.ReadByte();

					for (int i = 0; i < segmentCount; i++)
					{
						var segment = new PVS_SEGMENT_V1();
						segment.SegmentX = mem.ReadByte();
						segment.SegmentY = mem.ReadByte();
						var includeSegmentCount = mem.ReadUInt16();

						for (int f = 0; f < includeSegmentCount; f++)
						{
							var includeSegment = new SEGMENT_DATA_V1();
							includeSegment.SegmentX = mem.ReadByte();
							includeSegment.SegmentY = mem.ReadByte();
							segment.IncludeSegments.Add(includeSegment);
						}

						Segments.Add(segment);
					}

					for (int i = 0; i < propCount; i++)
					{
						var prop = new PVS_PROP_V1();
						prop.SegmentX = mem.ReadByte();
						prop.SegmentY = mem.ReadByte();
						var includePropCount = mem.ReadInt32();

						for (int f = 0; f < includePropCount; f++)
						{
							var includeProp = new PROP_DATA_V1();
							includeProp.PropIdx = mem.ReadUInt32();
							includeProp.SegmentIdx = mem.ReadUInt32();
							prop.IncludeProps.Add(includeProp);
						}

						Props.Add(prop);
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Pvs::Load<Exception> -> {exception}\n");
			}
		}

		/// <summary>
		/// Remove entry by index
		/// </summary>
		/// <param name="index"></param>
		public void RemoveP(int index)
		{
			Props.RemoveAt(index);
		}

		/// <summary>
		/// Remove entry by index
		/// </summary>
		/// <param name="index"></param>
		public void RemoveS(int index)
		{
			Segments.RemoveAt(index);
		}
	}
}
