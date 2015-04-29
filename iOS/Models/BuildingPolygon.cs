﻿using System;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
using Foundation;

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

		public static BuildingPolygon Create (CLLocationCoordinate2D[] coords)
		{
			return FromCoordinates (coords) as BuildingPolygon;
		}

		public NSValue[] CreatePath (string pathCoords)
		{
			var pointsMK = new List<NSValue> ();
			var points = pathCoords.Split ('|');
			foreach (var coord in points) {
				var point = coord.Split (", ".ToCharArray ());
				if (point.Length == 2) {
					float x = Convert.ToSingle (point [0]);
					float y = Convert.ToSingle (point [1]);
					var clCoord = new CLLocationCoordinate2D (x, y);
					pointsMK.Add (NSValue.FromMKCoordinate(clCoord));
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

