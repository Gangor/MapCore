using DataCore;
using MapCore.Enum;
using MapCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapCore
{
	public class Nfm
	{
		#region Stucture 

		public string Sign { get; set; } = "nFlavor Map\0\0\0\0\0";
		public int Version { get; set; } = 22;
		public int TileCountPerSegment { get; set; } = 6;
		public int SegmentCountPerMap { get; set; } = 64;
		public float TileLenght { get; set; } = 42.0f;
		public KProperties MapProperties { get; set; } = new KProperties();
		public KSegment[,] DwTerrainSegment { get; set; } = new KSegment[64, 64];
		public List<KProp> DwProps { get; set; } = new List<KProp>();
		public List<KGrass> DwGrass { get; set; } = new List<KGrass>();
		public List<Polygon2> DwVectorAttr { get; set; } = new List<Polygon2>();
		public List<Water> DwWater { get; set; } = new List<Water>();
		public List<SpeedGrassColony> DwGrassColony { get; set; } = new List<SpeedGrassColony>();
		public List<EventAreaScript> DwEventArea { get; set; } = new List<EventAreaScript>();

		#endregion

		public MapManager Parent;
		public int[] SupportedVersion = { 16, 17, 19, 21, 22 };

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public Nfm(MapManager module) {
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			TileCountPerSegment = 6;
			SegmentCountPerMap = 64;
			TileLenght = 42;
			MapProperties = new KProperties();
			DwTerrainSegment = new KSegment[64, 64];

			for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
				for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
				{
					DwTerrainSegment[segmentX, segmentY] = new KSegment();
					for (int titleY = 0; titleY < TileCountPerSegment; titleY++)
						for (int titleX = 0; titleX < TileCountPerSegment; titleX++)
						{
							DwTerrainSegment[segmentX, segmentY].HsVector[titleX, titleY] = new KVertex();
						}
				}

			DwProps.Clear();
			DwGrass.Clear();
			DwVectorAttr.Clear();
			DwWater.Clear();
			DwGrassColony.Clear();
			DwEventArea.Clear();
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
					mem.Write(22);

					mem.Write(0); /* dwMapPropertiesOffset = */
					mem.Write(0); /* dwTerrainSegmentOffset = */
					mem.Write(0); /* dwPropOffset = */
					mem.Write(0); /* dwVectorAttrOffset = */
					mem.Write(0); /* dwWaterOffset = */
					mem.Write(0); /* dwGrassColonyOffset = */
					mem.Write(0); /* dwEventAreaOffset = */

					mem.Write(TileCountPerSegment);
					mem.Write(SegmentCountPerMap);
					mem.Write(TileLenght);

					#region Properties

					int dwMapPropertiesOffset = (int)mem.Position;

					mem.Write(MapProperties.Primary.Diffuse.Color.B);
					mem.Write(MapProperties.Primary.Diffuse.Color.G);
					mem.Write(MapProperties.Primary.Diffuse.Color.R);
					mem.Write(MapProperties.Primary.Specular.Color.B);
					mem.Write(MapProperties.Primary.Specular.Color.G);
					mem.Write(MapProperties.Primary.Specular.Color.R);
					mem.Write(MapProperties.Primary.Attenuation0);
					mem.Write(MapProperties.Primary.Attenuation1);
					mem.Write(MapProperties.Primary.Attenuation2);
					mem.Write(MapProperties.Secondary.Diffuse.Color.B);
					mem.Write(MapProperties.Secondary.Diffuse.Color.G);
					mem.Write(MapProperties.Secondary.Diffuse.Color.R);
					mem.Write(MapProperties.Secondary.Specular.Color.B);
					mem.Write(MapProperties.Secondary.Specular.Color.G);
					mem.Write(MapProperties.Secondary.Specular.Color.R);
					mem.Write(MapProperties.Secondary.Attenuation0);
					mem.Write(MapProperties.Secondary.Attenuation1);
					mem.Write(MapProperties.Secondary.Attenuation2);
					mem.Write(MapProperties.Sky.Color.B);
					mem.Write(MapProperties.Sky.Color.G);
					mem.Write(MapProperties.Sky.Color.R);
					mem.Write(MapProperties.Fog.Color.B);
					mem.Write(MapProperties.Fog.Color.G);
					mem.Write(MapProperties.Fog.Color.R);
					mem.Write(MapProperties.FogNear);
					mem.Write(MapProperties.FogFar);
					mem.Write(MapProperties.SkyType);
					mem.Write(MapProperties.ShowTerrainInGame);

					#endregion

					#region Terrain segment

					int dwTerrainSegmentOffset = (int)mem.Position;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							mem.Write(DwTerrainSegment[segmentX, segmentY].Version);

							for (int tile = 0; tile < 3; tile++)
							{
								mem.Write(DwTerrainSegment[segmentX, segmentY].Tile[tile]);
							}

							for (int titleY = 0; titleY < TileCountPerSegment; titleY++)
								for (int tileX = 0; tileX < TileCountPerSegment; tileX++)
								{
									mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Height);

									for (int f = 0; f < 2; f++)
									{
										mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].FillBits[f]);
									}

									mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Attribute);
									mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.B);
									mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.G);
									mem.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.R);
								}
						}

					#endregion

					#region Prop

					int dwPropOffset = (int)mem.Position;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							mem.Write(0);
						}

					int index = 0;
					int segment = 0;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							var offset = (int)mem.Position;
							mem.Seek(dwPropOffset + (4 * segment), SeekOrigin.Begin);
							mem.Write(offset);
							mem.Seek(offset, SeekOrigin.Begin);

							var prop = DwProps.Where(r => r.SegmentId == segment).ToList();

							mem.Write(prop.Count);

							for (int p = 0; p < prop.Count; p++)
							{
								mem.Write(index);
								mem.Write(prop[p].X);
								mem.Write(prop[p].Y);
								mem.Write(prop[p].Z);
								mem.Write(prop[p].RotateX);
								mem.Write(prop[p].RotateY);
								mem.Write(prop[p].RotateZ);
								mem.Write(prop[p].ScaleX);
								mem.Write(prop[p].ScaleY);
								mem.Write(prop[p].ScaleZ);
								mem.Write(prop[p].PropNum);
								mem.Write(prop[p].HeightLocked);
								mem.Write(prop[p].LockHeight);
								mem.Write(prop[p].TextureGroup);
								index++;
							}

							var grass = DwGrass.Where(r => r.SegmentId == segment).ToList();

							mem.Write(grass.Count);

							for (int n = 0; n < grass.Count; n++)
							{
								mem.Write(grass[n].GrassId);
								mem.Write(grass[n].Props.Count);

								for (int i = 0; i < grass[n].Props.Count; i++)
								{
									mem.Write(grass[n].Props[i].X);
									mem.Write(grass[n].Props[i].Y);
									mem.Write(grass[n].Props[i].RotateX);
									mem.Write(grass[n].Props[i].RotateY);
									mem.Write(grass[n].Props[i].RotateZ);
								}
							}

							segment++;
						}

					#endregion

					#region Vector attribute

					int dwVectorAttrOffset = (int)mem.Position;

					mem.Write(Parent.Nfa.Polygons.Count);

					for (int i = 0; i < Parent.Nfa.Polygons.Count; i++)
					{
						mem.Write(Parent.Nfa.Polygons[i].Count);

						for (int p = 0; p < Parent.Nfa.Polygons[i].Count; p++)
						{
							mem.Write(Parent.Nfa.Polygons[i][p].X);
							mem.Write(Parent.Nfa.Polygons[i][p].Y);
						}
					}

					#endregion

					#region Water

					int dwWaterOffset = (int)mem.Position;

					mem.Write(DwWater.Count);

					for (int i = 0; i < DwWater.Count; i++)
					{
						foreach (var item in DwWater[i].Points)
						{
							mem.Write(item.X);
							mem.Write(item.Y);
							mem.Write(item.Z);
						}
						mem.Write(DwWater[i].UseReflect);
						mem.Write(DwWater[i].WaterId);
					}

					#endregion

					#region Speed grass

					int dwGrassColonyOffset = (int)mem.Position;

					mem.Write(DwGrassColony.Count);

					for (int i = 0; i < DwGrassColony.Count; i++)
					{
						mem.Write(i + 1);
						mem.Write(DwGrassColony[i].Density);
						mem.Write(DwGrassColony[i].Distribution);
						mem.Write(DwGrassColony[i].Size);
						mem.Write(DwGrassColony[i].HeightP);
						mem.Write(DwGrassColony[i].HeightM);
						mem.Write(DwGrassColony[i].Color.Color.B);
						mem.Write(DwGrassColony[i].Color.Color.G);
						mem.Write(DwGrassColony[i].Color.Color.R);
						mem.Write(DwGrassColony[i].Color.Color.A);
						mem.Write(DwGrassColony[i].ColorRatio);
						mem.Write(DwGrassColony[i].ColorTone);
						mem.Write(DwGrassColony[i].Chroma);
						mem.Write(DwGrassColony[i].Brightness);
						mem.Write(DwGrassColony[i].CombinationRatio);
						mem.Write(DwGrassColony[i].WindReaction);

						var texture = DwGrassColony[i].Filename.Length == 0 ?
							DwGrassColony[i].Filename :
							DwGrassColony[i].Filename.Replace("\0", "") + '\0';

						mem.Write(texture.Length);
						mem.Write(Encoding.Default.GetBytes(texture));
						mem.Write(DwGrassColony[i].Polygons.Count);

						for (int p = 0; p < DwGrassColony[i].Polygons.Count; p++)
						{
							mem.Write(DwGrassColony[i].Polygons[p].Count);

							for (int n = 0; n < DwGrassColony[i].Polygons[p].Count; n++)
							{
								mem.Write(DwGrassColony[i].Polygons[p][n].X);
								mem.Write(DwGrassColony[i].Polygons[p][n].Y);
							}
						}
					}

					#endregion

					#region Event area

					int dwEventAreaOffset = (int)mem.Position;

					mem.Write(Parent.Nfe.Events.Count);

					for (int i = 0; i < Parent.Nfe.Events.Count; i++)
					{
						mem.Write(Parent.Nfe.Events[i].AreaId);
						mem.Write(Parent.Nfe.Events[i].Polygons.Count);

						for (int p = 0; p < Parent.Nfe.Events[i].Polygons.Count; p++)
						{
							mem.Write(Parent.Nfe.Events[i].Polygons[p].Count);

							for (int n = 0; n < Parent.Nfe.Events[i].Polygons[p].Count; n++)
							{
								mem.Write(Parent.Nfe.Events[i].Polygons[p][n].X);
								mem.Write(Parent.Nfe.Events[i].Polygons[p][n].Y);
							}
						}
					}

					#endregion

					mem.Seek(20, SeekOrigin.Begin);
					mem.Write(dwMapPropertiesOffset);
					mem.Write(dwTerrainSegmentOffset);
					mem.Write(dwPropOffset);
					mem.Write(dwVectorAttrOffset);
					mem.Write(dwWaterOffset);
					mem.Write(dwGrassColonyOffset);
					mem.Write(dwEventAreaOffset);

					Parent.Log(Levels.Good, "Ok\n");
					return mem.ToArray();
				}

			}
			catch (Exception exception)
			{
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfm::GetBuffer<Exception> -> {exception}\n");
			}

			return null;
		}

		/// <summary>
		/// Load existing map
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
						Parent.Log(Levels.Error, $"Incompatible version {Version} is not supported or not implemented.\n");
						return;
					}
#endif

					var dwMapPropertiesOffset = mem.ReadInt32();
					var dwTerrainSegmentOffset = mem.ReadInt32();
					var dwPropOffset = mem.ReadInt32();
					var dwVectorAttrOffset = mem.ReadInt32();
					var dwWaterOffset = mem.ReadInt32();
					var dwGrassColonyOffset = (Version >= 17) ? mem.ReadInt32() : 0;
					var dwEventAreaOffset = (Version >= 22) ? mem.ReadInt32() : 0;

					TileCountPerSegment = mem.ReadInt32();
					SegmentCountPerMap = mem.ReadInt32();
					TileLenght = mem.ReadSingle();

	#region Properties
                    
					MapProperties.Primary.Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.Primary.Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.Primary.Attenuation0 = mem.ReadSingle();
					MapProperties.Primary.Attenuation1 = mem.ReadSingle();
					MapProperties.Primary.Attenuation2 = mem.ReadSingle();
					MapProperties.Secondary.Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.Secondary.Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.Secondary.Attenuation0 = mem.ReadSingle();
					MapProperties.Secondary.Attenuation1 = mem.ReadSingle();
					MapProperties.Secondary.Attenuation2 = mem.ReadSingle();
					MapProperties.Sky = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.Fog = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					MapProperties.FogNear = mem.ReadSingle();
					MapProperties.FogFar = mem.ReadSingle();
					MapProperties.SkyType = mem.ReadUInt32();
					MapProperties.ShowTerrainInGame = mem.ReadBoolean();

	#endregion

	#region Terrain segment

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							DwTerrainSegment[segmentX, segmentY] = new KSegment();
							DwTerrainSegment[segmentX, segmentY].Version = (Version >= 16) ? mem.ReadUInt32() : 0;

							for (int tile = 0; tile < 3; tile++)
							{
								DwTerrainSegment[segmentX, segmentY].Tile[tile] = (Version >= 16) ? mem.ReadUInt16() : (ushort)0;
							}

							DwTerrainSegment[segmentX, segmentY].HsVector = new KVertex[TileCountPerSegment, TileCountPerSegment];

							for (int tileY = 0; tileY < TileCountPerSegment; tileY++)
								for (int tileX = 0; tileX < TileCountPerSegment; tileX++)
								{
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY] = new KVertex();
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Height = mem.ReadSingle();

									for (int f = 0; f < 2; f++)
									{
										if (Version >= 16) DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = mem.ReadUInt32();
										if (Version == 15 && f == 0) DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = mem.ReadUInt32();
									}

									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Attribute = (Version >= 16) ? mem.ReadInt64() : 0;
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Color = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
								}
						}

	#endregion

	#region Prop
                    
					//Escape offset prop table
					for (int i = 0; i < SegmentCountPerMap * SegmentCountPerMap; i++)
					{
						mem.ReadInt32();
					}

					// PROP
					var segment = 0;
					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							var propcount = mem.ReadInt32();

							for (int p = 0; p < propcount; p++)
							{
								var prop = new KProp();
								/* index */ mem.ReadInt32();
								prop.SegmentId = segment;
								prop.X = mem.ReadSingle();
								prop.Y = mem.ReadSingle();
								prop.Z = mem.ReadSingle();
								prop.RotateX = mem.ReadSingle();
								prop.RotateY = mem.ReadSingle();
								prop.RotateZ = mem.ReadSingle();
								prop.ScaleX = mem.ReadSingle();
								prop.ScaleY = (Version >= 21) ? mem.ReadSingle() : prop.ScaleX;
								prop.ScaleZ = (Version >= 21) ? mem.ReadSingle() : prop.ScaleX;
								prop.PropNum = mem.ReadUInt16();
								prop.HeightLocked = (Version >= 21) ? mem.ReadBoolean() : false;
								prop.LockHeight = (Version >= 21) ? mem.ReadSingle() : 0.0f;
								prop.TextureGroup = (Version >= 21) ? mem.ReadInt16() : (short)-1;
								DwProps.Add(prop);
							}

							if (Version >= 19)
							{
								var grassCount = mem.ReadInt32();

								for (int n = 0; n < grassCount; n++)
								{
									var grass = new KGrass();
									grass.SegmentId = segment;
									grass.GrassId = mem.ReadInt32();
									var propCount = mem.ReadInt32();

									for (int i = 0; i < propCount; i++)
									{
										var prop = new KGrassProp();
										prop.X = mem.ReadSingle();
										prop.Y = mem.ReadSingle();
										prop.RotateX = mem.ReadSingle();
										prop.RotateY = mem.ReadSingle();
										prop.RotateZ = mem.ReadSingle();
										grass.Props.Add(prop);
									}

									DwGrass.Add(grass);
								}
							}

							segment++;
						}

	#endregion

	#region Vector attribute
                    
					var polygonCount = mem.ReadInt32();

					for (int i = 0; i < polygonCount; i++)
					{
						var polygon = new Polygon2();
						var pointNum = mem.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var point = new K2DPosition(mem.ReadInt32(), mem.ReadInt32());
							polygon.Add(point);
						}

						DwVectorAttr.Add(polygon);
					}

	#endregion

	#region Water

					var waterCount = mem.ReadInt32();
                    
					for (int i = 0; i < waterCount; i++)
					{
						var water = new Water();
						water.Points[0].X = mem.ReadSingle();
						water.Points[0].Y = mem.ReadSingle();
						water.Points[0].Z = mem.ReadSingle();
						water.Points[1].X = mem.ReadSingle();
						water.Points[1].Y = mem.ReadSingle();
						water.Points[1].Z = mem.ReadSingle();
						water.Points[2].X = mem.ReadSingle();
						water.Points[2].Y = mem.ReadSingle();
						water.Points[2].Z = mem.ReadSingle();
						water.UseReflect = mem.ReadInt32();
						water.WaterId = mem.ReadInt32();
						DwWater.Add(water);
					}

					#endregion
                    
					if (Version >= 17)
					{
	#region Speed grass

						var speedGrassCount = mem.ReadInt32();

						for (int i = 0; i < speedGrassCount; i++)
						{
							var speedGrass = new SpeedGrassColony();
							speedGrass.GrassId = mem.ReadInt32();
							speedGrass.Density = mem.ReadSingle();
							speedGrass.Distribution = mem.ReadSingle();
							speedGrass.Size = mem.ReadSingle();
							speedGrass.HeightP = mem.ReadSingle();
							speedGrass.HeightM = mem.ReadSingle();
							speedGrass.Color = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
							speedGrass.ColorRatio = mem.ReadSingle();
							speedGrass.ColorTone = mem.ReadSingle();
							speedGrass.Chroma = mem.ReadSingle();
							speedGrass.Brightness = mem.ReadSingle();
							speedGrass.CombinationRatio = mem.ReadSingle();
							speedGrass.WindReaction = mem.ReadSingle();
							speedGrass.Filename = Encoding.Default.GetString(mem.ReadBytes(mem.ReadInt32()));
							var polyshCount = mem.ReadInt32();

							for (int p = 0; p < polyshCount; p++)
							{
								var polygon = new Polygon2();
								var pointCount = mem.ReadInt32();
								for (int n = 0; n < pointCount; n++)
								{
									var point = new K2DPosition(mem.ReadInt32(), mem.ReadInt32());
									polygon.Add(point);
								}
								speedGrass.Polygons.Add(polygon);
							}
							DwGrassColony.Add(speedGrass);
						}

	#endregion
					}

					if (Version >= 22)
					{
	#region Event area

						var eventAreaCount = mem.ReadInt32();

						for (int i = 0; i < eventAreaCount; i++)
						{
							var area = new EventAreaScript();
							area.AreaId = mem.ReadInt32();
							var count = mem.ReadInt32();

							for (int p = 0; p < count; p++)
							{
								var polygon = new Polygon2();
								var pointNum = mem.ReadInt32();

								for (int n = 0; n < pointNum; n++)
								{
									var point = new K2DPosition(mem.ReadInt32(), mem.ReadInt32());
									polygon.Add(point);
								}

								area.Polygons.Add(polygon);
							}

							DwEventArea.Add(area);
						}

	#endregion
					}
				}

				Parent.Log(Levels.Good, "Ok\n");
			}
			catch (Exception exception)
			{
				Blank();
				Parent.Log(Levels.Error, "Failed\n");
				Parent.Log(Levels.Fatal, $"Nfm::Load<Exception> -> {exception}\n");
			}
		}
	}
}
