using System;
using System.Collections.Generic;
using System.Linq;

namespace RITMaps.iOS
{
	public class BuildingManager : BuildingManagerBase
	{
		public static List<BuildingAnnotation> Buildings { get; set; }

		static BuildingManager ()
		{
			ResourceLoader = BuildingFactory.Create ();
		}

		public static void LoadData()
		{
			var loadedBuildings = ResourceLoader.Load (ResourceFile.Markers);
			Buildings.AddRange (loadedBuildings.Cast<BuildingAnnotation>());
		}
	}
}

