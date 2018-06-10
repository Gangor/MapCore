#pragma warning disable CS1591

using System.Collections.Generic;

namespace MapCore.Models
{
    public class SpeedGrassColony
    {
        public int GrassId { get; set; }
        public float Density { get; set; }
        public float Distribution { get; set; }
        public float Size { get; set; }
        public float HeightP { get; set; }
        public float HeightM { get; set; }
		public KColor Color { get; set; } = new KColor();
        public float ColorRatio { get; set; }
        public float ColorTone { get; set; }
        public float Chroma { get; set; }
        public float Brightness { get; set; }
        public float CombinationRatio { get; set; }
        public float WindReaction { get; set; }
        public string Filename { get; set; }
		public List<Polygon> Polygons { get; set; } = new List<Polygon>();
    };
}
