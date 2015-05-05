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
		static string MarkerFileName { get; } = "all-rit-markers";

		static string PolygonFileName { get; } = "all-rit-polygons";

		static string TagFileName { get; } = "all-rit-tags";

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

