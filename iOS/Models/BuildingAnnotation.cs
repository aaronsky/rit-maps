using System;
using MapKit;
using CoreLocation;

namespace RITMaps.iOS
{
	public class BuildingAnnotation : MKAnnotation, IRITBuilding
	{
		public string Name { get; set; }

		public string BuildingId { get; set; }

		public string ShortDescription { get; set; }

		public string ImageUrl { get; set; }

		public string Abbreviation { get; set; }

		public string History { get; set; }

		public string FullDescription { get; set; }

		public string[] Tags { get; set; }

		string title;
		public override string Title { get { return title; } }

		string subtitle;
		public override string Subtitle { get { return subtitle; } }

		CLLocationCoordinate2D coordinate;
		public override CLLocationCoordinate2D Coordinate { get { return coordinate; } }

		public BuildingPolygon Boundaries { get; }

		public BuildingAnnotation (CLLocationCoordinate2D coord,
		                           string title,
		                           string subtitle)
		{
			coordinate = coord;
			this.title = title;
			this.subtitle = subtitle;
		}
	}
}

