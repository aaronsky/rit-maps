﻿namespace RITMaps
{
	public interface IRITBuilding
	{
		string Id { get; set; }

		string Name { get; set; }

		string BuildingId { get; set; }

		string ShortDescription { get; set; }

		string ImageUrl { get; set; }

		string Abbreviation { get; set; }

		string History { get; set; }

		string FullDescription { get; set; }

		string[] Tags { get; set; }
	}
}

