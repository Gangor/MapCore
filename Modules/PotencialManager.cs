using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Management for quest prop (Pvs)
	/// </summary>
	public class PotencialManager
	{
		/// <summary>
		/// Map slot X
		/// </summary>
		public byte MapStartPosX { get; set; }

		/// <summary>
		/// Map slot Y
		/// </summary>
		public byte MapStartPosY { get; set; }

		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent { get; }

		/// <summary>
		/// Get or set the list of all potencial prop
		/// </summary>
		public List<PotencialProp> Props { get; set; } = new List<PotencialProp>();

		/// <summary>
		/// Get or set the list of all potencial segment
		/// </summary>
		public List<PotencialSegment> Segments { get; set; } = new List<PotencialSegment>();

		/// <summary>
		/// Get or set segement count per map
		/// </summary>
		public byte SegmentCountPerMap { get; set; } = 64;

		/// <summary>
		/// Get or set segment left
		/// </summary>
		public byte SegmentLeft { get; set; }

		/// <summary>
		/// Get or set segment top
		/// </summary>
		public byte SegmentTop { get; set; }

		/// <summary>
		/// Get or set segment right
		/// </summary>
		public byte SegmentRight { get; set; }
		
		/// <summary>
		/// Get or set segment bottom
		/// </summary>
		public byte SegmentBottom { get; set; }

		/// <summary>
		/// Get the signature
		/// </summary>
		public string Sign { get; } = "RAPPELZ PVS\0\0\0\0\0";

		/// <summary>
		/// Get support version
		/// </summary>
		public int[] SupportedVersion { get; } = { 1 };

		/// <summary>
		/// Get or set the version
		/// </summary>
		public ushort Version { get; set; } = 1;

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public PotencialManager(MapCore parent)
		{
			Dispose();
			Parent = parent;
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Dispose()
		{
			SegmentCountPerMap = 64;
			MapStartPosX = 0;
			MapStartPosY = 0;
			SegmentLeft = 0;
			SegmentTop = 0;
			SegmentRight = 0;
			SegmentBottom = 0;
			Segments = new List<PotencialSegment>();
			Props = new List<PotencialProp>();
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
		public void Load(byte[] buffer)
		{
			try
			{
				using (MemoryReader mem = new MemoryReader(buffer))
				{
					/*Sign = Encoding.Default.GetString()*/ mem.ReadBytes(16);
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
						var segment = new PotencialSegment();
						segment.SegmentX = mem.ReadByte();
						segment.SegmentY = mem.ReadByte();
						var includeSegmentCount = mem.ReadUInt16();

						for (int f = 0; f < includeSegmentCount; f++)
						{
							var includeSegment = new PotencialSegmentData();
							includeSegment.SegmentX = mem.ReadByte();
							includeSegment.SegmentY = mem.ReadByte();
							segment.IncludeSegments.Add(includeSegment);
						}

						Segments.Add(segment);
					}

					for (int i = 0; i < propCount; i++)
					{
						var prop = new PotencialProp();
						prop.SegmentX = mem.ReadByte();
						prop.SegmentY = mem.ReadByte();
						var includePropCount = mem.ReadInt32();

						for (int f = 0; f < includePropCount; f++)
						{
							var includeProp = new PotencialPropData();
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
				Dispose();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Pvs::Load<Exception> -> {exception}\n");
			}
		}
	}
}
