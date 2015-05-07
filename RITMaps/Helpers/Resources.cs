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

		static string PolygonUrl { get; } = "http://maps.rit.edu/proxySearch/?q=*&wt=json&indent=off&fq=polygon_id:*&rows=1000&fl=name,mdo_id,description,longitude,latitude,path,abbreviation,building_number,image,tag";

		static string TagUrl { get; } = "http://maps.rit.edu/proxySearch/?q=*&wt=json&indent=on&facet=on&facet.field=tag&facet.mincount=1&rows=0";

		public static string ResourceFileToFileName (ResourceFile resource)
		{
			switch (resource) {
			case ResourceFile.Markers:
				return MarkerFileName;
			case ResourceFile.Polygons:
				return PolygonUrl;
			case ResourceFile.Tags:
				return TagUrl;
			default:
				return "";
			}
		}
	}
}

