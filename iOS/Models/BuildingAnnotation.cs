using System;
using MapKit;
using CoreLocation;

namespace RITMaps.iOS
{
	public class BuildingAnnotation : MKAnnotation
	{
		public RITBuilding Building { get; set; }

		public override string Title { get; }

		public override string Subtitle { get; }

		public override CLLocationCoordinate2D Coordinate { get; set; }

		public BuildingPolygon Boundaries { get; }

		public BuildingAnnotation (CLLocationCoordinate2D coord,
		                           string title,
		                           string subtitle)
		{
			Coordinate = coord;
			Title = title;
			Subtitle = subtitle;
		}
	}
}

