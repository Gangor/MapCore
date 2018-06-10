#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
	public class Terrain
	{
		public string Sign { get; } = "nFlavor Map\0\0\0\0\0";
		public TerrainProperties MapProperties { get; set; } = new TerrainProperties();
		public TerrainSegment[,] DwTerrainSegment { get; set; } = new TerrainSegment[64, 64];
		public List<TerrainProp> DwProps { get; set; } = new List<TerrainProp>();
		public List<GrassProp> DwGrass { get; set; } = new List<GrassProp>();
		public List<Polygon> DwVectorAttr { get; set; } = new List<Polygon>();
		public List<Water> DwWater { get; set; } = new List<Water>();
		public List<SpeedGrassColony> DwGrassColony { get; set; } = new List<SpeedGrassColony>();
		public List<EventArea> DwEventArea { get; set; } = new List<EventArea>();
		public int Version { get; set; } = 22;
	}
}
