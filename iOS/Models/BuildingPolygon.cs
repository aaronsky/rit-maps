using System;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
using Foundation;
using System.Linq;

namespace RITMaps.iOS
{
	public class BuildingPolygon : RITPolygon
	{
		public CLLocationCoordinate2D[] Path { get; set; }

		public MKPolygon Polygon {get;set;}

		BuildingPolygon ()
		{
		}

		public static BuildingPolygon Create (string polyId, CLLocationCoordinate2D[] coords, string[] tags)
		{
			var polygon = new BuildingPolygon {
				PolygonID = polyId,
				Path = coords,
				Polygon = MKPolygon.FromCoordinates (coords),
				Tags = tags
			};
			return polygon;
		}

		public static BuildingPolygon Create (string polyId, string coords, string[] tags)
		{
			if (coords == null)
				coords = string.Empty;
			return BuildingPolygon.Create (polyId, CreatePath(coords), tags);
		}

		public static CLLocationCoordinate2D[] CreatePath (string pathCoords)
		{
			var pointsMK = new List<CLLocationCoordinate2D> ();
			var points = pathCoords.Split ('|');
			foreach (var coord in points) {
				var point = coord.Split (", ".ToCharArray ());
				if (point.Length == 3) {
					float x = Convert.ToSingle (point [0]);
					float y = Convert.ToSingle (point [2]);
					pointsMK.Add (new CLLocationCoordinate2D (x, y));
				} else if (point.Length == 2) {
					float x = Convert.ToSingle (point [0]);
					float y = Convert.ToSingle (point [1]);
					pointsMK.Add (new CLLocationCoordinate2D (x, y));
				}
			}
			return pointsMK.ToArray ();
		}
	}
}

