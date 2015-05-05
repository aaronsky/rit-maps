using System;
using MapKit;
using CoreLocation;

namespace RITMaps.iOS
{
	public class BuildingAnnotation : MKAnnotation, IRITBuilding
	{
		public string Name { get; set; }

		public string BuildingId { get; set; }

		public string ImageUrl { get; set; }

		public string Abbreviation { get; set; }

		public string History { get; set; }

		public string FullDescription { get; set; }

		public string[] Tags { get; set; }

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

