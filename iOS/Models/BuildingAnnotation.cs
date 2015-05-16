using System;
using MapKit;
using CoreLocation;
using Newtonsoft.Json;

namespace RITMaps.iOS
{
	public class BuildingAnnotation : MKAnnotation, IRITBuilding
	{
		[JsonProperty ("mdo_id")]
		public string Id { get; set; }

		[JsonProperty ("name")]
		public string Name { get; set; }

		[JsonProperty ("building_number")]
		public string BuildingId { get; set; }

		[JsonProperty ("description")]
		public string ShortDescription { get; set; }

		[JsonProperty ("image")]
		public string ImageUrl { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

		[JsonProperty ("abbreviation")]
		public string Abbreviation { get; set; }

		[JsonProperty ("history")]
		public string History { get; set; }

		[JsonProperty ("full_description")]
		public string FullDescription { get; set; }

		[JsonProperty ("tag")]
		public string[] Tags { get; set; }

		string title;

		public override string Title { get { return title; } }

		string subtitle;

		public override string Subtitle { get { return subtitle; } }

		CLLocationCoordinate2D coordinate;

		public override CLLocationCoordinate2D Coordinate { get { return coordinate; } }

		public BuildingPolygon Boundaries { get; set; }

        public int Count { get; set; }

        public BuildingAnnotation(CLLocationCoordinate2D coord, int count) : this(coord, "", "")
        {
            Count = count;
        }

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

