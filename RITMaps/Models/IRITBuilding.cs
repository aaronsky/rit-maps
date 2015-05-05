namespace RITMaps
{
	public interface IRITBuilding
	{
		string Name { get; set; }

		string BuildingId { get; set; }

		string Description { get; set; }

		string ImageUrl { get; set; }

		string Abbreviation { get; set; }

		string History { get; set; }

		string FullDescription { get; set; }

		string[] Tags { get; set; }
	}
}

