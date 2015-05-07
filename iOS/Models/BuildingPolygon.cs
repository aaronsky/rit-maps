using System;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
using Foundation;
using System.Linq;

namespace RITMaps.iOS
{
	public class BuildingPolygon : MKPolygon
	{
		public string PolygonID { get; set; }

		public CLLocationCoordinate2D[] Path { get; set; }

		public bool IsSelected { get; set; }

		public bool IsInside { get; set; }

		public string[] Tags { get; set; }

		private BuildingPolygon ()
		{
		}

		public static BuildingPolygon Create (string polyId, CLLocationCoordinate2D[] coords, string[] tags)
		{
			var polygon =  FromCoordinates (coords) as BuildingPolygon;
			polygon.PolygonID = polyId;
			polygon.Tags = tags;
			return polygon;
		}

		public static BuildingPolygon Create (string polyId, string coords, string[] tags)
		{
			if (coords == null)
				return null;
			return BuildingPolygon.Create (polyId, CreatePath(coords), tags);
		}

		public static CLLocationCoordinate2D[] CreatePath (string pathCoords)
		{
			var pointsMK = new List<CLLocationCoordinate2D> ();
			var points = pathCoords.Split ('|');
			foreach (var coord in points) {
				var point = coord.Split (", ".ToCharArray ());
				if (point.Length == 2) {
					float x = Convert.ToSingle (point [0]);
					float y = Convert.ToSingle (point [1]);
					pointsMK.Add (new CLLocationCoordinate2D (x, y));
				}
			}
			return pointsMK.ToArray ();
		}

		public void PointInsidePolygon (CLLocationCoordinate2D point)
		{
			var mapPoint = MKMapPoint.FromCoordinate (point);
			var polygonView = new MKPolygonRenderer (this);
			var polygonViewPoint = polygonView.PointForMapPoint (mapPoint);
			IsInside = polygonView.Path.ContainsPoint (polygonViewPoint, false);
		}
	}
}

