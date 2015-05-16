namespace RITMaps.Helpers
{
	public enum ResourceFile
	{
		Markers,
		Polygons,
		Tags,
		Routing
	}

	public static class Resources
	{
		static string MarkerFileName { get { return "all-rit-markers"; } }

		static string PolygonUrl { get { return "all-rit-polygons"; } }
		//"http://maps.rit.edu/proxySearch/?q=*&wt=json&indent=off&fq=polygon_id:*&rows=1000&fl=name,mdo_id,description,longitude,latitude,path,abbreviation,building_number,image,tag";

		static string TagUrl { get { return "all-rit-tags"; } }
		//"http://maps.rit.edu/proxySearch/?q=*&wt=json&indent=on&facet=on&facet.field=tag&facet.mincount=1&rows=0";

        static string OsmUrl { get { return "rit"; } }

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

