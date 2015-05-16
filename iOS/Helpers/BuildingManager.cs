using CoreLocation;
using MapKit;
using RITMaps.Helpers;
using RITMaps.iOS.Extensions;
using RITMaps.Models;
using System;
using System.Collections.Generic;

namespace RITMaps.iOS
{
	public static class BuildingManager
	{
		public static QuadTree<BuildingAnnotation> Buildings { get; set; }
		public static Dictionary<int, string> Tags { get; set; }
		public static IResourceLoader ResourceLoader { get; set; }

		static BuildingManager ()
		{
			ResourceLoader = BuildingFactory.Create ();
			Tags = new Dictionary<int, string> ();
		}

        public static BuildingAnnotation[] ClusteredAnnotations(MKMapRect rect, double zoomScale)
        {
            var cellSize = CellSizeForZoomScale(zoomScale);
            var scaleFactor = zoomScale / cellSize;
            var minX = Math.Floor(rect.MinX * scaleFactor);
            var maxX = Math.Floor(rect.MaxX * scaleFactor);
            var minY = Math.Floor(rect.MinY * scaleFactor);
            var maxY = Math.Floor(rect.MaxY * scaleFactor);
            var clusteredAnnotations = new List<BuildingAnnotation>();
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    double totalX = 0;
                    double totalY = 0;
                    int count = 0;

                    Buildings.Gather(rect.ToBoundingBox(), (annotation) =>
                    {
                        totalX += annotation.Longitude;
                        totalY += annotation.Latitude;
                        count++;
                        clusteredAnnotations.Add(annotation);
                    });
                    if (count == 1)
                    {
                        var coordinate = new CLLocationCoordinate2D(totalY, totalX);
                        var annotation = new BuildingAnnotation(coordinate, count);
                        clusteredAnnotations.Add(annotation);
                    }
                    if (count > 1)
                    {
                        var coordinate = new CLLocationCoordinate2D(totalY / count, totalX / count);
                        var annotation = new BuildingAnnotation(coordinate, count);
                        clusteredAnnotations.Add(annotation);
                    }
                }
            }
            return clusteredAnnotations.ToArray();
        }

        static float CellSizeForZoomScale(double zoomScale)
        {
            var zoomLevel = ZoomScaleToZoomLevel(zoomScale);
            switch (zoomLevel)
            {
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

        static int ZoomScaleToZoomLevel(double scale)
        {
            var totalTilesAtMaxZoom = MKMapSize.World.Width / 256.0;
            var zoomLevelAtMaxZoom = Math.Log(totalTilesAtMaxZoom, 2);
            int zoomLevel = (int)Math.Max(0, zoomLevelAtMaxZoom + Math.Floor(Math.Log(scale, 2) + 0.5));
            return zoomLevel;
        }
	}
}

