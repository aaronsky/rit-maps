namespace RITMaps
{
	public class RITBuilding
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string BuildingId { get; set; }

		public string ShortDescription { get; set; }

		public string ImageUrl { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string Abbreviation { get; set; }

		public string History { get; set; }

		public string FullDescription { get; set; }

		public string[] Tags { get; set; }

		public RITPolygon Boundaries { get; set; }
	}
}

