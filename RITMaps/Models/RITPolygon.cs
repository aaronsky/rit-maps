using System;

namespace RITMaps
{
	public class RITPolygon
	{
		public string PolygonID { get; set; }

		public bool IsSelected { get; set; }

		public bool IsInside { get; set; }

		public string[] Tags { get; set; }
	}
}

