namespace MapCore
{
	/// <summary>
	/// Global variable
	/// </summary>
	public class Global
	{
		/// <summary>
		/// Get or set the map lenght
		/// </summary>
		public static int Lenght { get; } = 16128;

		/// <summary>
		/// Get or set the picture lenght
		/// </summary>
		public static int PictureLenght { get; } = 8064;

		/// <summary>
		/// Get or set the map lenght
		/// </summary>
		public static int TerrainLenght => (int)(SegmentCountPerMap * TileCountPerSegment * TileLenght);

		/// <summary>
		/// Get the ratio of the point
		/// </summary>
		public static float AttrLenght { get; } = 0.125f;

		/// <summary>
		/// Get or set the segment count
		/// </summary>
		public static int SegmentCountPerMap { get; set; } = 64;

		/// <summary>
		/// Get or set the tile count
		/// </summary>
		public static int TileCountPerSegment { get; set; } = 6;

		/// <summary>
		/// Get or set the tile lenght
		/// </summary>
		public static float TileLenght { get; set; } = 42.0f;
	}
}
