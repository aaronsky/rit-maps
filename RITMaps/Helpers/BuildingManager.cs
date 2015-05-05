using System;
using System.Linq;
using System.Collections.Generic;

namespace RITMaps
{
	public static class BuildingManager
	{
		public static List<IRITBuilding> Buildings { get; set; }

		static readonly IResourceLoader resourceLoader;

		static BuildingManager ()
		{
			resourceLoader = BuildingFactory.Create ();
			Buildings = resourceLoader.Load (ResourceFile.Markers).ToList();
		}
	}
}

