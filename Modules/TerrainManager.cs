using MapCore.Enum;
using MapCore.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Management for terrain (Nfm)
	/// </summary>
	public class TerrainManager
	{
		/// <summary>
		/// Get the parent module
		/// </summary>
		public MapCore Parent;

		/// <summary>
		/// Get the terrain
		/// </summary>
		public Terrain Terrain = new Terrain();

		/// <summary>
		/// Get support version
		/// </summary>
		public int[] SupportedVersion { get; } = { 16, 17, 19, 21, 22 };

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public TerrainManager(MapCore module) {
			Blank();
			Parent = module;
		}

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			Terrain.TileCountPerSegment = 6;
			Terrain.SegmentCountPerMap = 64;
			Terrain.TileLenght = 42;
			Terrain.MapProperties = new TerrainProperties();
			Terrain.DwTerrainSegment = new TerrainSegment[64, 64];

			for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
				for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
				{
					Terrain.DwTerrainSegment[segmentX, segmentY] = new TerrainSegment();
					for (int titleY = 0; titleY < Terrain.TileCountPerSegment; titleY++)
						for (int titleX = 0; titleX < Terrain.TileCountPerSegment; titleX++)
						{
							Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[titleX, titleY] = new TerrainVertex();
						}
				}

			Terrain.DwProps.Clear();
			Terrain.DwGrass.Clear();
			Terrain.DwVectorAttr.Clear();
			Terrain.DwWater.Clear();
			Terrain.DwGrassColony.Clear();
			Terrain.DwEventArea.Clear();
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
					mem.Write(Encoding.Default.GetBytes(Terrain.Sign));
					mem.Write(22);

					mem.Write(0); /* dwMapPropertiesOffset = */
					mem.Write(0); /* dwTerrainSegmentOffset = */
					mem.Write(0); /* dwPropOffset = */
					mem.Write(0); /* dwVectorAttrOffset = */
					mem.Write(0); /* dwWaterOffset = */
					mem.Write(0); /* dwGrassColonyOffset = */
					mem.Write(0); /* dwEventAreaOffset = */

					mem.Write(Terrain.TileCountPerSegment);
					mem.Write(Terrain.SegmentCountPerMap);
					mem.Write(Terrain.TileLenght);

					#region Properties

					int dwMapPropertiesOffset = (int)mem.Position;

					mem.Write(Terrain.MapProperties.Primary.Diffuse.Color.B);
					mem.Write(Terrain.MapProperties.Primary.Diffuse.Color.G);
					mem.Write(Terrain.MapProperties.Primary.Diffuse.Color.R);
					mem.Write(Terrain.MapProperties.Primary.Specular.Color.B);
					mem.Write(Terrain.MapProperties.Primary.Specular.Color.G);
					mem.Write(Terrain.MapProperties.Primary.Specular.Color.R);
					mem.Write(Terrain.MapProperties.Primary.Attenuation0);
					mem.Write(Terrain.MapProperties.Primary.Attenuation1);
					mem.Write(Terrain.MapProperties.Primary.Attenuation2);
					mem.Write(Terrain.MapProperties.Secondary.Diffuse.Color.B);
					mem.Write(Terrain.MapProperties.Secondary.Diffuse.Color.G);
					mem.Write(Terrain.MapProperties.Secondary.Diffuse.Color.R);
					mem.Write(Terrain.MapProperties.Secondary.Specular.Color.B);
					mem.Write(Terrain.MapProperties.Secondary.Specular.Color.G);
					mem.Write(Terrain.MapProperties.Secondary.Specular.Color.R);
					mem.Write(Terrain.MapProperties.Secondary.Attenuation0);
					mem.Write(Terrain.MapProperties.Secondary.Attenuation1);
					mem.Write(Terrain.MapProperties.Secondary.Attenuation2);
					mem.Write(Terrain.MapProperties.Sky.Color.B);
					mem.Write(Terrain.MapProperties.Sky.Color.G);
					mem.Write(Terrain.MapProperties.Sky.Color.R);
					mem.Write(Terrain.MapProperties.Fog.Color.B);
					mem.Write(Terrain.MapProperties.Fog.Color.G);
					mem.Write(Terrain.MapProperties.Fog.Color.R);
					mem.Write(Terrain.MapProperties.FogNear);
					mem.Write(Terrain.MapProperties.FogFar);
					mem.Write(Terrain.MapProperties.SkyType);
					mem.Write(Terrain.MapProperties.ShowTerrainInGame);

					#endregion

					#region Terrain segment

					int dwTerrainSegmentOffset = (int)mem.Position;

					for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
						{
							mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].Version);

							for (int tile = 0; tile < 3; tile++)
							{
								mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].Tile[tile]);
							}

							for (int titleY = 0; titleY < Terrain.TileCountPerSegment; titleY++)
								for (int tileX = 0; tileX < Terrain.TileCountPerSegment; tileX++)
								{
									mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Height);

									for (int f = 0; f < 2; f++)
									{
										mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].FillBits[f]);
									}

									mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Attribute);
									mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.B);
									mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.G);
									mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.R);
								}
						}

					#endregion

					#region Prop

					int dwPropOffset = (int)mem.Position;

					for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
						{
							mem.Write(0);
						}

					int index = 0;
					int segment = 0;

					for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
						{
							var offset = (int)mem.Position;
							mem.Seek(dwPropOffset + (4 * segment), SeekOrigin.Begin);
							mem.Write(offset);
							mem.Seek(offset, SeekOrigin.Begin);

							var prop = Terrain.DwProps.Where(r => r.SegmentId == segment).ToList();

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

							var grass = Terrain.DwGrass.Where(r => r.SegmentId == segment).ToList();

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

					var dwVectorAttrOffset = (int)mem.Position;
					var collisions = Parent == null ? Terrain.DwVectorAttr : Parent.Nfa.Polygons;

					mem.Write(collisions.Count);

					for (int i = 0; i < collisions.Count; i++)
					{
						mem.Write(collisions[i].Count);

						for (int p = 0; p < collisions[i].Count; p++)
						{
							mem.Write((int)collisions[i][p].X);
							mem.Write((int)collisions[i][p].Y);
						}
					}

					#endregion

					#region Water

					int dwWaterOffset = (int)mem.Position;

					mem.Write(Terrain.DwWater.Count);

					for (int i = 0; i < Terrain.DwWater.Count; i++)
					{
						foreach (var item in Terrain.DwWater[i].Points)
						{
							mem.Write(item.X);
							mem.Write(item.Y);
							mem.Write(item.Z);
						}
						mem.Write(Terrain.DwWater[i].UseReflect);
						mem.Write(Terrain.DwWater[i].WaterId);
					}

					#endregion

					#region Speed grass

					int dwGrassColonyOffset = (int)mem.Position;

					mem.Write(Terrain.DwGrassColony.Count);

					for (int i = 0; i < Terrain.DwGrassColony.Count; i++)
					{
						mem.Write(i + 1);
						mem.Write(Terrain.DwGrassColony[i].Density);
						mem.Write(Terrain.DwGrassColony[i].Distribution);
						mem.Write(Terrain.DwGrassColony[i].Size);
						mem.Write(Terrain.DwGrassColony[i].HeightP);
						mem.Write(Terrain.DwGrassColony[i].HeightM);
						mem.Write(Terrain.DwGrassColony[i].Color.Color.B);
						mem.Write(Terrain.DwGrassColony[i].Color.Color.G);
						mem.Write(Terrain.DwGrassColony[i].Color.Color.R);
						mem.Write(Terrain.DwGrassColony[i].Color.Color.A);
						mem.Write(Terrain.DwGrassColony[i].ColorRatio);
						mem.Write(Terrain.DwGrassColony[i].ColorTone);
						mem.Write(Terrain.DwGrassColony[i].Chroma);
						mem.Write(Terrain.DwGrassColony[i].Brightness);
						mem.Write(Terrain.DwGrassColony[i].CombinationRatio);
						mem.Write(Terrain.DwGrassColony[i].WindReaction);

						var texture = Terrain.DwGrassColony[i].Filename.Length == 0 ?
							Terrain.DwGrassColony[i].Filename :
							Terrain.DwGrassColony[i].Filename.Replace("\0", "") + '\0';

						mem.Write(texture.Length);
						mem.Write(Encoding.Default.GetBytes(texture));
						mem.Write(Terrain.DwGrassColony[i].Polygons.Count);

						for (int p = 0; p < Terrain.DwGrassColony[i].Polygons.Count; p++)
						{
							mem.Write(Terrain.DwGrassColony[i].Polygons[p].Count);

							for (int n = 0; n < Terrain.DwGrassColony[i].Polygons[p].Count; n++)
							{
								mem.Write((int)Terrain.DwGrassColony[i].Polygons[p][n].X);
								mem.Write((int)Terrain.DwGrassColony[i].Polygons[p][n].Y);
							}
						}
					}

					#endregion

					#region Event area

					int dwEventAreaOffset = (int)mem.Position;
					var eventareas = Parent == null ? Terrain.DwEventArea : Parent.Nfe.Areas;

					mem.Write(eventareas.Count);

					for (int i = 0; i < eventareas.Count; i++)
					{
						mem.Write(eventareas[i].AreaId);
						mem.Write(eventareas[i].Polygons.Count);

						for (int p = 0; p < eventareas[i].Polygons.Count; p++)
						{
							mem.Write(eventareas[i].Polygons[p].Count);

							for (int n = 0; n < eventareas[i].Polygons[p].Count; n++)
							{
								mem.Write((int)eventareas[i].Polygons[p][n].X);
								mem.Write((int)eventareas[i].Polygons[p][n].Y);
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
					/*Terrain.Sign = Encoding.Default.GetString()*/ mem.ReadBytes(16);
					Terrain.Version = mem.ReadInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Terrain.Version))
					{
						Parent.Log(Levels.Error, $"Failed\n");
						Parent.Log(Levels.Error, $"Incompatible version {Terrain.Version} is not supported or not implemented.\n");
						return;
					}
#endif

					var dwMapPropertiesOffset = mem.ReadInt32();
					var dwTerrainSegmentOffset = mem.ReadInt32();
					var dwPropOffset = mem.ReadInt32();
					var dwVectorAttrOffset = mem.ReadInt32();
					var dwWaterOffset = mem.ReadInt32();
					var dwGrassColonyOffset = (Terrain.Version >= 17) ? mem.ReadInt32() : 0;
					var dwEventAreaOffset = (Terrain.Version >= 22) ? mem.ReadInt32() : 0;

					Terrain.TileCountPerSegment = mem.ReadInt32();
					Terrain.SegmentCountPerMap = mem.ReadInt32();
					Terrain.TileLenght = mem.ReadSingle();

					#region Properties

					Terrain.MapProperties.Primary.Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.Primary.Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.Primary.Attenuation0 = mem.ReadSingle();
					Terrain.MapProperties.Primary.Attenuation1 = mem.ReadSingle();
					Terrain.MapProperties.Primary.Attenuation2 = mem.ReadSingle();
					Terrain.MapProperties.Secondary.Diffuse = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.Secondary.Specular = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.Secondary.Attenuation0 = mem.ReadSingle();
					Terrain.MapProperties.Secondary.Attenuation1 = mem.ReadSingle();
					Terrain.MapProperties.Secondary.Attenuation2 = mem.ReadSingle();
					Terrain.MapProperties.Sky = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.Fog = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte(), 255);
					Terrain.MapProperties.FogNear = mem.ReadSingle();
					Terrain.MapProperties.FogFar = mem.ReadSingle();
					Terrain.MapProperties.SkyType = mem.ReadUInt32();
					Terrain.MapProperties.ShowTerrainInGame = mem.ReadBoolean();

	#endregion

	#region Terrain segment

					for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
						{
							Terrain.DwTerrainSegment[segmentX, segmentY] = new TerrainSegment();
							Terrain.DwTerrainSegment[segmentX, segmentY].Version = (Terrain.Version >= 16) ? mem.ReadUInt32() : 0;

							for (int tile = 0; tile < 3; tile++)
							{
								Terrain.DwTerrainSegment[segmentX, segmentY].Tile[tile] = (Terrain.Version >= 16) ? mem.ReadUInt16() : (ushort)0;
							}

							Terrain.DwTerrainSegment[segmentX, segmentY].HsVector = new TerrainVertex[Terrain.TileCountPerSegment, Terrain.TileCountPerSegment];

							for (int tileY = 0; tileY < Terrain.TileCountPerSegment; tileY++)
								for (int tileX = 0; tileX < Terrain.TileCountPerSegment; tileX++)
								{
									Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY] = new TerrainVertex();
									Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Height = mem.ReadSingle();

									for (int f = 0; f < 2; f++)
									{
										if (Terrain.Version >= 16) Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = mem.ReadUInt32();
										if (Terrain.Version == 15 && f == 0) Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = mem.ReadUInt32();
									}

									Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Attribute = (Terrain.Version >= 16) ? mem.ReadInt64() : 0;
									Terrain.DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Color = new KColor(mem.ReadByte(), mem.ReadByte(), mem.ReadByte());
								}
						}

	#endregion

	#region Prop
                    
					//Escape offset prop table
					for (int i = 0; i < Terrain.SegmentCountPerMap * Terrain.SegmentCountPerMap; i++)
					{
						mem.ReadInt32();
					}

					// PROP
					var segment = 0;
					for (int segmentY = 0; segmentY < Terrain.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Terrain.SegmentCountPerMap; segmentX++)
						{
							var propcount = mem.ReadInt32();

							for (int p = 0; p < propcount; p++)
							{
								var prop = new TerrainProp();
								/* index */ mem.ReadInt32();
								prop.SegmentId = segment;
								prop.X = mem.ReadSingle();
								prop.Y = mem.ReadSingle();
								prop.Z = mem.ReadSingle();
								prop.RotateX = mem.ReadSingle();
								prop.RotateY = mem.ReadSingle();
								prop.RotateZ = mem.ReadSingle();
								prop.ScaleX = mem.ReadSingle();
								prop.ScaleY = (Terrain.Version >= 21) ? mem.ReadSingle() : prop.ScaleX;
								prop.ScaleZ = (Terrain.Version >= 21) ? mem.ReadSingle() : prop.ScaleX;
								prop.PropNum = mem.ReadUInt16();
								prop.HeightLocked = (Terrain.Version >= 21) ? mem.ReadBoolean() : false;
								prop.LockHeight = (Terrain.Version >= 21) ? mem.ReadSingle() : 0.0f;
								prop.TextureGroup = (Terrain.Version >= 21) ? mem.ReadInt16() : (short)-1;
								Terrain.DwProps.Add(prop);
							}

							if (Terrain.Version >= 19)
							{
								var grassCount = mem.ReadInt32();

								for (int n = 0; n < grassCount; n++)
								{
									var grass = new Grass();
									grass.SegmentId = segment;
									grass.GrassId = mem.ReadInt32();
									var propCount = mem.ReadInt32();

									for (int i = 0; i < propCount; i++)
									{
										var prop = new GrassProp();
										prop.X = mem.ReadSingle();
										prop.Y = mem.ReadSingle();
										prop.RotateX = mem.ReadSingle();
										prop.RotateY = mem.ReadSingle();
										prop.RotateZ = mem.ReadSingle();
										grass.Props.Add(prop);
									}

									Terrain.DwGrass.Add(grass);
								}
							}

							segment++;
						}

	#endregion

	#region Vector attribute
                    
					var polygonCount = mem.ReadInt32();

					for (int i = 0; i < polygonCount; i++)
					{
						var polygon = new Polygon();
						var pointNum = mem.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var point = new Vector(mem.ReadInt32(), mem.ReadInt32());
							polygon.Add(point);
						}

						Terrain.DwVectorAttr.Add(polygon);
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
						Terrain.DwWater.Add(water);
					}

					#endregion
                    
					if (Terrain.Version >= 17)
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
								var polygon = new Polygon();
								var pointCount = mem.ReadInt32();
								for (int n = 0; n < pointCount; n++)
								{
									var point = new Vector(mem.ReadInt32(), mem.ReadInt32());
									polygon.Add(point);
								}
								speedGrass.Polygons.Add(polygon);
							}
							Terrain.DwGrassColony.Add(speedGrass);
						}

	#endregion
					}

					if (Terrain.Version >= 22)
					{
	#region Event area

						var eventAreaCount = mem.ReadInt32();

						for (int i = 0; i < eventAreaCount; i++)
						{
							var area = new EventArea();
							area.AreaId = mem.ReadInt32();
							var count = mem.ReadInt32();

							for (int p = 0; p < count; p++)
							{
								var polygon = new Polygon();
								var pointNum = mem.ReadInt32();

								for (int n = 0; n < pointNum; n++)
								{
									var point = new Vector(mem.ReadInt32(), mem.ReadInt32());
									polygon.Add(point);
								}

								area.Polygons.Add(polygon);
							}

							Terrain.DwEventArea.Add(area);
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
