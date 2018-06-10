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
		/// Get the ratio of the point
		/// </summary>
		public int PointRatio { get; } = 8;

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
			Global.TileCountPerSegment = 6;
			Global.SegmentCountPerMap = 64;
			Global.TileLenght = 42;
			Terrain.MapProperties = new TerrainProperties();
			Terrain.DwTerrainSegment = new TerrainSegment[64, 64];

			for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
				for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
				{
					Terrain.DwTerrainSegment[segmentX, segmentY] = new TerrainSegment();
					for (int titleY = 0; titleY < Global.TileCountPerSegment; titleY++)
						for (int titleX = 0; titleX < Global.TileCountPerSegment; titleX++)
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

					mem.Write(Global.TileCountPerSegment);
					mem.Write(Global.SegmentCountPerMap);
					mem.Write(Global.TileLenght);

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

					for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
						{
							mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].Version);

							for (int tile = 0; tile < 3; tile++)
							{
								mem.Write(Terrain.DwTerrainSegment[segmentX, segmentY].Tile[tile]);
							}

							for (int titleY = 0; titleY < Global.TileCountPerSegment; titleY++)
								for (int tileX = 0; tileX < Global.TileCountPerSegment; tileX++)
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

					for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
						{
							mem.Write(0);
						}

					int index = 0;
					int segment = 0;

					for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
						{
							var offset = (int)mem.Position;
							mem.Seek(dwPropOffset + (4 * segment), SeekOrigin.Begin);
							mem.Write(offset);
							mem.Seek(offset, SeekOrigin.Begin);

							var prop = Terrain.DwProps.Where(r => r.Position.GetSegmentId() == segment).ToList();

							mem.Write(prop.Count);

							for (int p = 0; p < prop.Count; p++)
							{
								mem.Write(index);

								var vector = prop[p].Position.Clone();

								vector = vector.GetSegmentCoordonate();

								mem.Write(vector.X);
								mem.Write(vector.Y);
								mem.Write(vector.Z);
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

							var grass = Terrain.DwGrass.Where(r => r.Position.GetSegmentId() == segment)
								.Select(u => u.GrassId)
								.Distinct()
								.ToList();

							mem.Write(grass.Count);

							for (int n = 0; n < grass.Count; n++)
							{
								var props = Terrain.DwGrass.Where(r => r.Position.GetSegmentId() == segment && r.GrassId == grass[n])
									.ToList();

								mem.Write(grass[n]);
								mem.Write(props.Count);

								for (int i = 0; i < props.Count; i++)
								{
									mem.Write(props[i].Position.X);
									mem.Write(props[i].Position.Y);
									mem.Write(props[i].RotateX);
									mem.Write(props[i].RotateY);
									mem.Write(props[i].RotateZ);
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
							var vector = collisions[i][p].Clone();

							vector.X = vector.X * Global.Scale * PointRatio / Global.TileLenght;
							vector.Y = vector.Y * Global.Scale * PointRatio / Global.TileLenght;
							vector = vector.Rotate180FlipY();

							mem.Write((int)vector.X);
							mem.Write((int)vector.Y);
						}
					}

					#endregion

					#region Water

					int dwWaterOffset = (int)mem.Position;

					mem.Write(Terrain.DwWater.Count);

					for (int i = 0; i < Terrain.DwWater.Count; i++)
					{
						var rectangle = Terrain.DwWater[i].Rectangle.Clone();

						rectangle.LeftTop.X = rectangle.LeftTop.X * Global.Scale;
						rectangle.LeftTop.Y = rectangle.LeftTop.Y * Global.Scale;
						rectangle.LeftTop = rectangle.LeftTop.Rotate180FlipY();

						rectangle.RightBottom.X = rectangle.RightBottom.X * Global.Scale;
						rectangle.RightBottom.Y = rectangle.RightBottom.Y * Global.Scale;
						rectangle.RightBottom = rectangle.RightBottom.Rotate180FlipY();

						rectangle.Center.X = rectangle.Center.X * Global.Scale;
						rectangle.Center.Y = rectangle.Center.Y * Global.Scale;
						rectangle.Center = rectangle.RightBottom.Rotate180FlipY();

						mem.Write((int)rectangle.LeftTop.X);
						mem.Write((int)rectangle.LeftTop.Y);
						mem.Write((int)rectangle.LeftTop.Z);
						mem.Write((int)rectangle.RightBottom.X);
						mem.Write((int)rectangle.RightBottom.Y);
						mem.Write((int)rectangle.RightBottom.Z);
						mem.Write((int)rectangle.Center.X);
						mem.Write((int)rectangle.Center.Y);
						mem.Write((int)rectangle.Center.Z);

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
								var vector = eventareas[i].Polygons[p][n].Clone();

								vector.X = vector.X * Global.Scale * PointRatio / Global.TileLenght;
								vector.Y = vector.Y * Global.Scale * PointRatio / Global.TileLenght;
								vector = vector.Rotate180FlipY();

								mem.Write((int)vector.X);
								mem.Write((int)vector.Y);
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

					Global.TileCountPerSegment = mem.ReadInt32();
					Global.SegmentCountPerMap = mem.ReadInt32();
					Global.TileLenght = mem.ReadSingle();

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

					for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
						{
							Terrain.DwTerrainSegment[segmentX, segmentY] = new TerrainSegment();
							Terrain.DwTerrainSegment[segmentX, segmentY].Version = (Terrain.Version >= 16) ? mem.ReadUInt32() : 0;

							for (int tile = 0; tile < 3; tile++)
							{
								Terrain.DwTerrainSegment[segmentX, segmentY].Tile[tile] = (Terrain.Version >= 16) ? mem.ReadUInt16() : (ushort)0;
							}

							Terrain.DwTerrainSegment[segmentX, segmentY].HsVector = new TerrainVertex[Global.TileCountPerSegment, Global.TileCountPerSegment];

							for (int tileY = 0; tileY < Global.TileCountPerSegment; tileY++)
								for (int tileX = 0; tileX < Global.TileCountPerSegment; tileX++)
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
					for (int i = 0; i < Global.SegmentCountPerMap * Global.SegmentCountPerMap; i++)
					{
						mem.ReadInt32();
					}

					// PROP
					var segment = 0;
					for (int segmentY = 0; segmentY < Global.SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < Global.SegmentCountPerMap; segmentX++)
						{
							var propcount = mem.ReadInt32();

							for (int p = 0; p < propcount; p++)
							{
								var prop = new TerrainProp();
								/* index */ mem.ReadInt32();

								var vector = new Vector
								{
									X = mem.ReadSingle() + (segmentX * Global.TileLenght * Global.TileCountPerSegment),
									Y = mem.ReadSingle() + (segmentY * Global.TileLenght * Global.TileCountPerSegment),
									Z = mem.ReadSingle()
								};

								prop.Position = vector;
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
									var grass = new GrassProp();
									var grassId = mem.ReadInt32();
									var propCount = mem.ReadInt32();

									for (int i = 0; i < propCount; i++)
									{
										var prop = new GrassProp();
										var vector = new Vector(mem.ReadSingle(), mem.ReadSingle());

										prop.GrassId = grassId;
										prop.Position = vector;
										prop.RotateX = mem.ReadSingle();
										prop.RotateY = mem.ReadSingle();
										prop.RotateZ = mem.ReadSingle();
										Terrain.DwGrass.Add(grass);
									}
									
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

						water.Rectangle.LeftTop = new Vector
						{
							X = mem.ReadSingle() / Global.Scale,
							Y = mem.ReadSingle() / Global.Scale,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

						water.Rectangle.RightBottom = new Vector
						{
							X = mem.ReadSingle() / Global.Scale,
							Y = mem.ReadSingle() / Global.Scale,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

						water.Rectangle.Center = new Vector
						{
							X = mem.ReadSingle() / Global.Scale,
							Y = mem.ReadSingle() / Global.Scale,
							Z = mem.ReadSingle()
						}
						.Rotate180FlipY();

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
									var vector = new Vector
									{
										X = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale,
										Y = mem.ReadInt32() * Global.TileLenght / PointRatio / Global.Scale
									};
									polygon.Add(vector.Rotate180FlipY());
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
