using System;
using MapKit;
using CoreLocation;
using Newtonsoft.Json;

namespace RITMaps.iOS
{
	public class BuildingAnnotation : MKAnnotation
	{
		public RITBuilding Building { get; set; }

		string title;

		public override string Title { get { return title; } }

		string subtitle;

		public override string Subtitle { get { return subtitle; } }

		CLLocationCoordinate2D coordinate;

		public override CLLocationCoordinate2D Coordinate { get { return coordinate; } }

		public BuildingPolygon Boundaries { get; set; }

		public BuildingAnnotation (RITBuilding building) : this (
				new CLLocationCoordinate2D (building.Latitude, building.Longitude),
				building.Name,
				building.ShortDescription)
		{
			Building = building;
		}

		public BuildingAnnotation (CLLocationCoordinate2D coord,
		                           string title = "",
		                           string subtitle = "")
		{
			coordinate = coord;
			this.title = title;
			this.subtitle = subtitle;
		}
	}
}

