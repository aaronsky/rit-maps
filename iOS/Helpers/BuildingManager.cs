using System;
using System.Collections.Generic;

namespace RITMaps.iOS
{
	public class BuildingManager : BuildingManagerPCL
	{
		public static List<BuildingAnnotation> Buildings { get; set; }

		static BuildingManager()
		{
			LoadData ();
		}

		public static async void LoadData ()
		{
			BuildingAnnotation[] loadedBuildings = (BuildingAnnotation[])await ResourceLoader.Load (ResourceFile.Markers);
			Buildings.AddRange (loadedBuildings);
		}

	}
}

