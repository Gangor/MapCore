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
		public static int PictureLenght { get; } = 2048;

		/// <summary>
		/// Get or set the map lenght
		/// </summary>
		public static int TerrainLenght => (int)(SegmentCountPerMap * TileCountPerSegment * TileLenght);

		/// <summary>
		/// Get or set the coordonate scale for conversion
		/// </summary>
		public static float ScaleRatio { get; } = 7.875f;

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
