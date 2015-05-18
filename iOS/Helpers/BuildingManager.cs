using CoreLocation;
using MapKit;
using RITMaps.Helpers;
using RITMaps.iOS.Extensions;
using RITMaps.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RITMaps.iOS
{
	public static class BuildingManager
	{
		public static QuadTree<RITBuilding> Buildings { get; set; }

		public static Dictionary<int, string> Tags { get; set; }

		public static IResourceLoader ResourceLoader { get; set; }

		static BuildingManager ()
		{
			ResourceLoader = BuildingFactory.Create ();
			Tags = new Dictionary<int, string> ();
		}

		public static BuildingAnnotation[] ClusteredAnnotations (MKMapRect rect, double zoomScale)
		{
			var cellSize = CellSizeForZoomScale (zoomScale) * 1.5;
			var scaleFactor = zoomScale / cellSize;
			var minX = Math.Floor (rect.MinX * scaleFactor);
			var maxX = Math.Floor (rect.MaxX * scaleFactor);
			var minY = Math.Floor (rect.MinY * scaleFactor);
			var maxY = Math.Floor (rect.MaxY * scaleFactor);
			var clusteredAnnotations = new List<BuildingAnnotation> ();
			for (var x = minX; x <= maxX; x++) {
				for (var y = minY; y <= maxY; y++) {
					var mapRect = new MKMapRect (x / scaleFactor, y / scaleFactor, 1.0 / scaleFactor, 1.0 / scaleFactor);
					double totalLatitude = 0;
					double totalLongitude = 0;

					var tempBuildings = new List<RITBuilding> ();

					Buildings.Gather (mapRect.ToBoundingBox (), (building) => {
						totalLongitude += building.Longitude;
						totalLatitude += building.Latitude;
						tempBuildings.Add (building);
					});
					var count = tempBuildings.Count;
					if (count == 1) {
						var building = tempBuildings.LastOrDefault ();
						if (building != null) {
							var annotation = new BuildingAnnotation (building);
							clusteredAnnotations.Add (annotation);
						}
					} else if (count > 1) {
						var coordinate = new CLLocationCoordinate2D (totalLatitude / (double)count, totalLongitude / (double)count);
						var annotationCluster = new BuildingAnnotationCluster (coordinate, tempBuildings);
						clusteredAnnotations.Add (annotationCluster);
					}
				}
			}
			return clusteredAnnotations.ToArray ();
		}

		static float CellSizeForZoomScale (double zoomScale)
		{
			var zoomLevel = ZoomScaleToZoomLevel (zoomScale);
			switch (zoomLevel) {
			case 13:
			case 14:
			case 15:
				return 64;
			case 16:
			case 17:
			case 18:
				return 32;
			case 19:
				return 16;
			default:
				return 88;
			}
		}

		static int ZoomScaleToZoomLevel (double scale)
		{
			var totalTilesAtMaxZoom = MKMapSize.World.Width / 256.0;
			var zoomLevelAtMaxZoom = Math.Log (totalTilesAtMaxZoom, 2);
			int zoomLevel = (int)Math.Max (0, zoomLevelAtMaxZoom + Math.Floor (Math.Log (scale, 2) + 0.5));
			return zoomLevel;
		}
	}
}

