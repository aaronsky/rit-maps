using System;

namespace RITMaps
{
	public enum ResourceFile
	{
		Markers,
		Polygons,
		Tags
	}

	public static class Resources
	{
		static string MarkerFileName { get; } = "all-rit-markers.js";

		static string PolygonFileName { get; } = "all-rit-polygons.js";

		static string TagFileName { get; } = "all-rit-tags.js";

		public static string ResourceFileToFileName (ResourceFile resource)
		{
			switch (resource) {
			case ResourceFile.Markers:
				return MarkerFileName;
			case ResourceFile.Polygons:
				return PolygonFileName;
			case ResourceFile.Tags:
				return TagFileName;
			default:
				return "";
			}
		}
	}
}

